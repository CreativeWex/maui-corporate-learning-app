using LmsApp.Infrastructure;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class LessonService : ILessonService
{
    private readonly IApiClient _api;
    private readonly ILocalRepository _repo;
    private readonly IGamificationService _gamification;

    public LessonService(IApiClient api, ILocalRepository repo, IGamificationService gamification)
    {
        _api = api;
        _repo = repo;
        _gamification = gamification;
    }

    public async Task<Lesson?> GetLessonAsync(int id)
    {
        var cached = await _repo.GetLessonByIdAsync(id);
        if (cached != null) return cached.ToDomain();
        return await _api.GetLessonAsync(id);
    }

    public async Task<bool> IsCompletedByUserAsync(int userId, int moduleId)
    {
        if (userId <= 0 || moduleId <= 0) return false;
        var progress = await _repo.GetUserModuleProgressByIdAsync(userId, moduleId);
        return progress?.Status == (int)ModuleStatus.Completed;
    }

    public async Task MarkCompletedAsync(int lessonId, int moduleId, int courseId, int userId)
    {
        await _repo.MarkLessonCompletedAsync(lessonId);
        await _repo.UpsertUserModuleProgressAsync(userId, moduleId, (int)ModuleStatus.Completed);

        // Unlock next module for this user
        var modules = await _repo.GetModulesByCourseIdAsync(courseId);
        var current = modules.FirstOrDefault(m => m.Id == moduleId);
        if (current != null)
        {
            var next = modules.FirstOrDefault(m => m.OrderIndex == current.OrderIndex + 1);
            if (next != null)
            {
                var nextProgress = await _repo.GetUserModuleProgressByIdAsync(userId, next.Id);
                if (nextProgress == null || nextProgress.Status == (int)ModuleStatus.Locked)
                    await _repo.UpsertUserModuleProgressAsync(userId, next.Id, (int)ModuleStatus.NotStarted);
            }
        }

        await _gamification.AwardXpAsync(userId, 10);
        await _gamification.CheckAndAwardAchievementsAsync(userId);
    }

    public async Task SaveSessionAsync(LearningSession session)
    {
        await _repo.SaveLearningSessionAsync(new LearningSessionEntity
        {
            UserId = session.UserId,
            LessonId = session.LessonId,
            StartedAt = session.StartedAt.ToString("O"),
            CompletedAt = session.CompletedAt?.ToString("O"),
            TimeSpentSec = session.TimeSpentSec
        });
    }
}
