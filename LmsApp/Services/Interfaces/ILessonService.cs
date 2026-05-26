using LmsApp.Models.Domain;

namespace LmsApp.Services.Interfaces;

public interface ILessonService
{
    Task<Lesson?> GetLessonAsync(int id);
    Task<bool> IsCompletedByUserAsync(int userId, int moduleId);
    Task MarkCompletedAsync(int lessonId, int moduleId, int courseId, int userId);
    Task SaveSessionAsync(LearningSession session);
}
