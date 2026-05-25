using LmsApp.Models.Domain;
using LmsApp.Infrastructure.Entities;

namespace LmsApp.Infrastructure;

public interface ILocalRepository
{
    Task InitAsync();

    // Users
    Task<UserEntity?> GetUserByEmailAsync(string email);
    Task<UserEntity?> GetUserByIdAsync(int id);
    Task SaveUserAsync(UserEntity user);
    Task<List<UserEntity>> GetUsersByManagerIdAsync(int managerId);

    // Courses
    Task<List<CourseEntity>> GetAllCoursesAsync();
    Task<CourseEntity?> GetCourseByIdAsync(int id);
    Task SaveCourseAsync(CourseEntity course);

    // Modules
    Task<List<ModuleEntity>> GetModulesByCourseIdAsync(int courseId);
    Task SaveModuleAsync(ModuleEntity module);
    Task UpdateModuleStatusAsync(int moduleId, int status);

    // Lessons
    Task<LessonEntity?> GetLessonByIdAsync(int id);
    Task<LessonEntity?> GetLessonByModuleIdAsync(int moduleId);
    Task SaveLessonAsync(LessonEntity lesson);
    Task MarkLessonCompletedAsync(int lessonId);

    // Quizzes
    Task<QuizEntity?> GetQuizByIdAsync(int id);
    Task<QuizEntity?> GetQuizByModuleIdAsync(int moduleId);
    Task<List<QuestionEntity>> GetQuestionsByQuizIdAsync(int quizId);
    Task SaveQuizAsync(QuizEntity quiz);
    Task SaveQuestionAsync(QuestionEntity question);
    Task SaveQuizResultAsync(QuizResultEntity result);
    Task<List<QuizResultEntity>> GetQuizResultsByUserAsync(int userId, int quizId);
    Task<List<QuizResultEntity>> GetAllQuizResultsByUserAsync(int userId);
    Task<int> GetAttemptCountAsync(int userId, int quizId);

    // Assignments
    Task<List<AssignmentEntity>> GetAssignmentsByUserIdAsync(int userId);
    Task<AssignmentEntity?> GetAssignmentAsync(int userId, int courseId);
    Task SaveAssignmentAsync(AssignmentEntity assignment);

    // Certificates
    Task<List<CertificateEntity>> GetCertificatesByUserIdAsync(int userId);
    Task<CertificateEntity?> GetCertificateAsync(int userId, int courseId);
    Task SaveCertificateAsync(CertificateEntity cert);

    // Achievements
    Task<List<AchievementEntity>> GetAllAchievementsAsync();
    Task<List<UserAchievementEntity>> GetUserAchievementsAsync(int userId);
    Task SaveAchievementAsync(AchievementEntity achievement);
    Task SaveUserAchievementAsync(UserAchievementEntity ua);

    // Learning Sessions
    Task SaveLearningSessionAsync(LearningSessionEntity session);
    Task<List<LearningSessionEntity>> GetSessionsByUserAsync(int userId, DateTime from, DateTime to);
    Task<List<LearningSessionEntity>> GetSessionsByUserAsync(int userId);

    // Leaderboard
    Task<List<LeaderboardEntryEntity>> GetLeaderboardAsync();
    Task SaveLeaderboardEntryAsync(LeaderboardEntryEntity entry);
    Task UpdateLeaderboardXpAsync(int userId, int xp);
}
