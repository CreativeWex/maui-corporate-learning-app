using LmsApp.Models.Domain;

namespace LmsApp.Services.Interfaces;

public interface IQuizService
{
    Task<Quiz?> GetQuizAsync(int id);
    Task<QuizResult> CalculateResultAsync(int quizId, int userId, List<Answer> answers, int timeSpentSec);
    Task<QuizResult?> SubmitResultAsync(QuizResult result);
    Task<int> GetAttemptCountAsync(int userId, int quizId);
    Task<int> GetAttemptsLeftAsync(int userId, int quizId, int maxAttempts);
}
