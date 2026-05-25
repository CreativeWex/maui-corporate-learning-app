using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Models.Dto;
using LmsApp.Models.Enums;

namespace LmsApp.Infrastructure;

public class MockApiClient : IApiClient
{
    private readonly ILocalRepository _repo;
    private readonly Random _rng = new();

    public bool SimulateErrors { get; set; }

    public MockApiClient(ILocalRepository repo) => _repo = repo;

    async Task Delay() => await Task.Delay(_rng.Next(150, 400));

    async Task ThrowIfError()
    {
        await Delay();
        if (SimulateErrors) throw new Exception("Ошибка сети (симуляция)");
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        await ThrowIfError();
        var user = await _repo.GetUserByEmailAsync(request.Email);
        if (user == null || user.PasswordHash != request.Password) return null;
        return new AuthResponse
        {
            Token = $"mock_token_{user.Id}_{Guid.NewGuid():N}",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            User = user.ToDomain()
        };
    }

    public async Task LogoutAsync() => await Delay();

    public async Task<List<Course>> GetCoursesAsync()
    {
        await Delay();
        var courses = await _repo.GetAllCoursesAsync();
        return courses.Select(c => c.ToDomain()).ToList();
    }

    public async Task<CourseDetail?> GetCourseDetailAsync(int id)
    {
        await Delay();
        var course = await _repo.GetCourseByIdAsync(id);
        if (course == null) return null;
        var modules = await _repo.GetModulesByCourseIdAsync(id);
        return new CourseDetail
        {
            Id = course.Id,
            Title = course.Title,
            Category = course.Category,
            CoverUrl = course.CoverUrl,
            AuthorName = course.AuthorName,
            DurationMinutes = course.DurationMinutes,
            Rating = course.Rating,
            Description = course.Description,
            IsNew = course.IsNew,
            Modules = modules.Select(m => m.ToDomain()).ToList()
        };
    }

    public async Task<Lesson?> GetLessonAsync(int id)
    {
        await Delay();
        var lesson = await _repo.GetLessonByIdAsync(id);
        return lesson?.ToDomain();
    }

    public async Task MarkLessonCompletedAsync(int lessonId)
    {
        await Delay();
        await _repo.MarkLessonCompletedAsync(lessonId);
    }

    public async Task<Quiz?> GetQuizAsync(int id)
    {
        await Delay();
        var quiz = await _repo.GetQuizByIdAsync(id);
        if (quiz == null) return null;
        var questions = await _repo.GetQuestionsByQuizIdAsync(id);
        var domain = quiz.ToDomain();
        domain.Questions = questions.Select(q => q.ToDomain()).ToList();
        return domain;
    }

    public async Task<QuizResult?> SubmitQuizResultAsync(QuizResult result)
    {
        await Delay();
        await _repo.SaveQuizResultAsync(new QuizResultEntity
        {
            QuizId = result.QuizId,
            UserId = result.UserId,
            Score = result.Score,
            PassedPercent = result.PassedPercent,
            Passed = result.Passed,
            AttemptNumber = result.AttemptNumber,
            TimeSpentSeconds = result.TimeSpentSeconds,
            CompletedAt = result.CompletedAt.ToString("O")
        });
        return result;
    }

    public async Task<List<Assignment>> GetAssignmentsAsync(int userId)
    {
        await Delay();
        var list = await _repo.GetAssignmentsByUserIdAsync(userId);
        return list.Select(a => a.ToDomain()).ToList();
    }

    public async Task<Assignment?> CreateAssignmentAsync(Assignment assignment)
    {
        await Delay();
        var entity = new AssignmentEntity
        {
            CourseId = assignment.CourseId,
            UserId = assignment.UserId,
            AssignedById = assignment.AssignedById,
            DeadlineDate = assignment.DeadlineDate.ToString("O"),
            AssignedAt = assignment.AssignedAt.ToString("O"),
            IsMandatory = assignment.IsMandatory,
            Message = assignment.Message,
            ReminderDaysJson = System.Text.Json.JsonSerializer.Serialize(assignment.ReminderDays)
        };
        await _repo.SaveAssignmentAsync(entity);
        assignment.Id = entity.Id;
        return assignment;
    }

    public async Task<List<Certificate>> GetCertificatesAsync(int userId)
    {
        await Delay();
        var list = await _repo.GetCertificatesByUserIdAsync(userId);
        return list.Select(c => c.ToDomain()).ToList();
    }

    public async Task<Certificate?> GetCertificateAsync(int userId, int courseId)
    {
        await Delay();
        var cert = await _repo.GetCertificateAsync(userId, courseId);
        return cert?.ToDomain();
    }

    public async Task<List<Achievement>> GetAchievementsAsync(int userId)
    {
        await Delay();
        var all = await _repo.GetAllAchievementsAsync();
        var unlocked = await _repo.GetUserAchievementsAsync(userId);
        var unlockedMap = unlocked.ToDictionary(ua => ua.AchievementId, ua => DateTime.Parse(ua.UnlockedAt));

        return all.Select(a => new Achievement
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            IconUrl = a.IconUrl,
            XpReward = a.XpReward,
            Condition = a.Condition,
            Progress = a.Progress,
            UnlockedAt = unlockedMap.TryGetValue(a.Id, out var dt) ? dt : null
        }).ToList();
    }

    public async Task<UserLevel?> GetUserLevelAsync(int userId)
    {
        await Delay();
        var user = await _repo.GetUserByIdAsync(userId);
        if (user == null) return null;

        var xpThresholds = new[] { 0, 100, 250, 500, 800, 1200, 1700, 2300, 3000, 4000 };
        var levelNames = new[] { "Новичок", "Стажёр", "Помощник", "Специалист", "Эксперт", "Профи", "Мастер", "Наставник", "Гуру", "Легенда" };

        int level = user.Level;
        int xp = user.TotalXp;
        int currentBase = level < xpThresholds.Length ? xpThresholds[level] : xpThresholds[^1];
        int nextBase = level + 1 < xpThresholds.Length ? xpThresholds[level + 1] : xpThresholds[^1] + 1000;

        return new UserLevel
        {
            Level = level,
            LevelName = level < levelNames.Length ? levelNames[level] : "Легенда",
            CurrentXp = xp - currentBase,
            XpToNext = nextBase - currentBase
        };
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
    {
        await Delay();
        var list = await _repo.GetLeaderboardAsync();
        return list.Select((e, i) => e.ToDomain(i + 1, 0)).ToList();
    }

    public async Task<ProgressStats?> GetProgressStatsAsync(int userId, string period)
    {
        await Delay();
        return new ProgressStats
        {
            Period = Enum.TryParse<StatPeriod>(period, out var p) ? p : StatPeriod.Month,
            CompletedCourses = 1,
            TotalHours = 2.5,
            CurrentStreak = 3,
            MaxStreak = 7,
            Rank = 6
        };
    }

    public async Task<List<LearningSession>> GetLearningSessionsAsync(int userId)
    {
        await Delay();
        var list = await _repo.GetSessionsByUserAsync(userId);
        return list.Select(s => s.ToDomain()).ToList();
    }
}
