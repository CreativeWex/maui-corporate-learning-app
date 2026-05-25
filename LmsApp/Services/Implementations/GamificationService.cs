using LmsApp.Infrastructure;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class GamificationService : IGamificationService
{
    private readonly IApiClient _api;
    private readonly ILocalRepository _repo;

    public GamificationService(IApiClient api, ILocalRepository repo)
    {
        _api = api;
        _repo = repo;
    }

    public async Task<List<Achievement>> GetAchievementsAsync(int userId)
        => await _api.GetAchievementsAsync(userId);

    public async Task<UserLevel?> GetUserLevelAsync(int userId)
        => await _api.GetUserLevelAsync(userId);

    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync(LeaderboardScope scope, StatPeriod period, int currentUserId)
    {
        var all = await _repo.GetLeaderboardAsync();
        var result = new List<LeaderboardEntry>();

        // Team scope: filter by same department as current user
        if (scope == LeaderboardScope.Team)
        {
            var currentUser = await _repo.GetUserByIdAsync(currentUserId);
            if (currentUser != null)
            {
                var teamUsers = (await _repo.GetUsersByManagerIdAsync(currentUser.ManagerId ?? currentUserId))
                    .Select(u => u.Id).ToHashSet();
                teamUsers.Add(currentUserId);
                all = all.Where(e => teamUsers.Contains(e.UserId)).ToList();
            }
        }

        for (int i = 0; i < all.Count; i++)
            result.Add(all[i].ToDomain(i + 1, currentUserId));

        return result;
    }

    public async Task AwardXpAsync(int userId, int xp)
    {
        var user = await _repo.GetUserByIdAsync(userId);
        if (user == null) return;
        user.TotalXp += xp;
        RecalculateLevel(user);
        await _repo.SaveUserAsync(user);
        await _repo.UpdateLeaderboardXpAsync(userId, user.TotalXp);
    }

    public async Task CheckAndAwardAchievementsAsync(int userId)
    {
        var sessions = await _repo.GetSessionsByUserAsync(userId);
        var unlocked = (await _repo.GetUserAchievementsAsync(userId)).Select(ua => ua.AchievementId).ToHashSet();

        // "Первый шаг" — хотя бы 1 урок
        if (sessions.Any() && !unlocked.Contains(1))
            await UnlockAchievement(userId, 1);

        // "Быстрый старт" — урок в первый день (сид: уже выдано)
        var user = await _repo.GetUserByIdAsync(userId);
        if (user != null)
        {
            // "Знаток ИБ" — завершён курс 3
            var ibResult = await _repo.GetQuizResultsByUserAsync(userId, 3);
            if (ibResult.Any(r => r.Passed) && !unlocked.Contains(6))
                await UnlockAchievement(userId, 6);
        }
    }

    async Task UnlockAchievement(int userId, int achievementId)
    {
        await _repo.SaveUserAchievementAsync(new UserAchievementEntity
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.Now.ToString("O")
        });
        var achievement = (await _repo.GetAllAchievementsAsync()).FirstOrDefault(a => a.Id == achievementId);
        if (achievement != null)
            await AwardXpAsync(userId, achievement.XpReward);
    }

    static void RecalculateLevel(UserEntity user)
    {
        var thresholds = new[] { 0, 100, 250, 500, 800, 1200, 1700, 2300, 3000, 4000 };
        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (user.TotalXp >= thresholds[i]) { user.Level = i; break; }
        }
    }
}
