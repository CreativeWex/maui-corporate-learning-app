using LmsApp.Models.Domain;
using LmsApp.Models.Dto;

namespace LmsApp.Infrastructure;

public interface IApiClient
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();

    Task<List<Course>> GetCoursesAsync();
    Task<CourseDetail?> GetCourseDetailAsync(int id);

    Task<Lesson?> GetLessonAsync(int id);
    Task MarkLessonCompletedAsync(int lessonId);

    Task<Quiz?> GetQuizAsync(int id);
    Task<QuizResult?> SubmitQuizResultAsync(QuizResult result);

    Task<List<Assignment>> GetAssignmentsAsync(int userId);
    Task<Assignment?> CreateAssignmentAsync(Assignment assignment);

    Task<List<Certificate>> GetCertificatesAsync(int userId);
    Task<Certificate?> GetCertificateAsync(int userId, int courseId);

    Task<List<Achievement>> GetAchievementsAsync(int userId);
    Task<UserLevel?> GetUserLevelAsync(int userId);
    Task<List<LeaderboardEntry>> GetLeaderboardAsync();

    Task<ProgressStats?> GetProgressStatsAsync(int userId, string period);
    Task<List<LearningSession>> GetLearningSessionsAsync(int userId);

    bool SimulateErrors { get; set; }
}
