using LmsApp.Infrastructure;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly IApiClient _api;
    private readonly ILocalRepository _repo;

    public CourseService(IApiClient api, ILocalRepository repo)
    {
        _api = api;
        _repo = repo;
    }

    public async Task<List<Course>> GetCatalogAsync(int userId = 0, string? filter = null, string? search = null)
    {
        var courses = await _api.GetCoursesAsync();

        if (!string.IsNullOrWhiteSpace(filter) && filter != "Все")
            courses = courses.Where(c => c.Category == filter).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLowerInvariant();
            courses = courses.Where(c =>
                c.Title.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
                c.AuthorName.Contains(lower, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var ordered = courses.OrderByDescending(c => c.IsAssigned)
                             .ThenByDescending(c => c.IsNew)
                             .ThenByDescending(c => c.Rating)
                             .ToList();

        if (userId > 0)
        {
            foreach (var c in ordered)
                c.ProgressPercent = await GetCourseProgressAsync(userId, c.Id);
        }

        return ordered;
    }

    public async Task<CourseDetail?> GetCourseDetailAsync(int id, int userId)
    {
        var detail = await _api.GetCourseDetailAsync(id);
        if (detail == null) return null;

        var userProgress = await _repo.GetUserModuleProgressAsync(userId, id);
        if (!userProgress.Any())
        {
            foreach (var m in detail.Modules.OrderBy(m => m.OrderIndex))
                m.Status = m.OrderIndex == 0 ? ModuleStatus.NotStarted : ModuleStatus.Locked;
        }
        else
        {
            var map = userProgress.ToDictionary(p => p.ModuleId, p => p.Status);
            foreach (var m in detail.Modules)
                m.Status = map.TryGetValue(m.Id, out var s) ? (ModuleStatus)s : ModuleStatus.Locked;
        }

        return detail;
    }

    public async Task<List<Course>> GetContinueLearningAsync(int userId)
    {
        var assignments = await _repo.GetAssignmentsByUserIdAsync(userId);
        var courses = await _api.GetCoursesAsync();

        var assignedIds = assignments.Select(a => a.CourseId).ToHashSet();
        var result = new List<Course>();

        foreach (var course in courses.Where(c => assignedIds.Contains(c.Id)))
        {
            var progress = await GetCourseProgressAsync(userId, course.Id);
            if (progress > 0 && progress < 100)
            {
                course.ProgressPercent = progress;
                course.IsAssigned = true;
                var assignment = assignments.FirstOrDefault(a => a.CourseId == course.Id);
                course.DeadlineDate = assignment != null ? DateTime.Parse(assignment.DeadlineDate) : null;
                result.Add(course);
            }
        }

        return result.OrderByDescending(c => c.LastAccessedAt).ToList();
    }

    public async Task<List<Course>> GetNewAssignmentsAsync(int userId)
    {
        var assignments = await _repo.GetAssignmentsByUserIdAsync(userId);
        var cutoff = DateTime.Now.AddDays(-7);
        var recent = assignments.Where(a => DateTime.Parse(a.AssignedAt) >= cutoff).ToList();

        if (!recent.Any()) return new();

        var courses = await _api.GetCoursesAsync();
        var result = new List<Course>();
        foreach (var assignment in recent.Take(2))
        {
            var course = courses.FirstOrDefault(c => c.Id == assignment.CourseId);
            if (course != null)
            {
                course.IsAssigned = true;
                course.DeadlineDate = DateTime.Parse(assignment.DeadlineDate);
                result.Add(course);
            }
        }
        return result;
    }

    public async Task<int> GetCourseProgressAsync(int userId, int courseId)
    {
        var modules = await _repo.GetModulesByCourseIdAsync(courseId);
        if (!modules.Any()) return 0;
        var userProgress = await _repo.GetUserModuleProgressAsync(userId, courseId);
        if (!userProgress.Any()) return 0;
        var map = userProgress.ToDictionary(p => p.ModuleId, p => p.Status);
        int completed = modules.Count(m => map.TryGetValue(m.Id, out var s) && s == (int)ModuleStatus.Completed);
        return (int)Math.Round((double)completed / modules.Count * 100);
    }
}
