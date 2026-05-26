using LmsApp.Models.Domain;

namespace LmsApp.Services.Interfaces;

public interface ICourseService
{
    Task<List<Course>> GetCatalogAsync(int userId = 0, string? filter = null, string? search = null);
    Task<CourseDetail?> GetCourseDetailAsync(int id, int userId);
    Task<List<Course>> GetContinueLearningAsync(int userId);
    Task<List<Course>> GetNewAssignmentsAsync(int userId);
    Task<int> GetCourseProgressAsync(int userId, int courseId);
}
