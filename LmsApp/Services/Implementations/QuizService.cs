using LmsApp.Infrastructure;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class QuizService : IQuizService
{
    private readonly IApiClient _api;
    private readonly ILocalRepository _repo;
    private readonly IGamificationService _gamification;
    private readonly ICertificateService _certService;

    public QuizService(IApiClient api, ILocalRepository repo, IGamificationService gamification, ICertificateService certService)
    {
        _api = api;
        _repo = repo;
        _gamification = gamification;
        _certService = certService;
    }

    public async Task<Quiz?> GetQuizAsync(int id)
    {
        var quiz = await _api.GetQuizAsync(id);
        if (quiz == null) return null;
        // Randomize option order (keep correct index tracking)
        foreach (var q in quiz.Questions)
        {
            if (q.Type == QuestionType.TrueFalse) continue;
            var zipped = q.Options.Select((opt, i) => (opt, i)).ToList();
            var shuffled = zipped.OrderBy(_ => Guid.NewGuid()).ToList();
            q.Options = shuffled.Select(x => x.opt).ToList();
            q.CorrectOptionIndices = q.CorrectOptionIndices
                .Select(ci => shuffled.FindIndex(x => x.i == ci))
                .ToList();
        }
        return quiz;
    }

    public Task<QuizResult> CalculateResultAsync(int quizId, int userId, List<Answer> answers, int timeSpentSec)
    {
        int correct = answers.Count(a => a.IsCorrect);
        int total = answers.Count(a => !a.IsSkipped) > 0 ? answers.Count : 1;
        int percent = (int)Math.Round((double)correct / Math.Max(answers.Count, 1) * 100);

        return Task.FromResult(new QuizResult
        {
            QuizId = quizId,
            UserId = userId,
            Score = correct,
            PassedPercent = percent,
            Passed = percent >= 80,
            AttemptNumber = 1,
            TimeSpentSeconds = timeSpentSec,
            AnswerDetails = answers,
            CompletedAt = DateTime.Now
        });
    }

    public async Task<QuizResult?> SubmitResultAsync(QuizResult result)
    {
        var attempt = await _repo.GetAttemptCountAsync(result.UserId, result.QuizId) + 1;
        result.AttemptNumber = attempt;

        await _api.SubmitQuizResultAsync(result);

        if (result.Passed)
        {
            var quizEntity = await _repo.GetQuizByIdAsync(result.QuizId);
            if (quizEntity != null)
            {
                await _repo.UpsertUserModuleProgressAsync(result.UserId, quizEntity.ModuleId, (int)ModuleStatus.Completed);

                var modules = await _repo.GetModulesByCourseIdAsync(quizEntity.CourseId);
                var current = modules.FirstOrDefault(m => m.Id == quizEntity.ModuleId);
                if (current != null)
                {
                    var next = modules.FirstOrDefault(m => m.OrderIndex == current.OrderIndex + 1);
                    if (next != null)
                    {
                        var nextProgress = await _repo.GetUserModuleProgressByIdAsync(result.UserId, next.Id);
                        if (nextProgress == null || nextProgress.Status == (int)ModuleStatus.Locked)
                            await _repo.UpsertUserModuleProgressAsync(result.UserId, next.Id, (int)ModuleStatus.NotStarted);
                    }
                }

                if (quizEntity.IsFinal)
                    await _certService.GenerateAsync(result.UserId, quizEntity.CourseId);
            }
        }

        int xp = result.PassedPercent switch
        {
            >= 100 => 100,
            >= 90 => 75,
            >= 80 => 50,
            _ => 20
        };
        await _gamification.AwardXpAsync(result.UserId, xp);
        await _gamification.CheckAndAwardAchievementsAsync(result.UserId);
        return result;
    }

    public async Task<int> GetAttemptCountAsync(int userId, int quizId)
        => await _repo.GetAttemptCountAsync(userId, quizId);

    public async Task<int> GetAttemptsLeftAsync(int userId, int quizId, int maxAttempts)
        => Math.Max(0, maxAttempts - await _repo.GetAttemptCountAsync(userId, quizId));
}
