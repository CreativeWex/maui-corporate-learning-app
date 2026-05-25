using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Services.Interfaces;

public interface IGamificationService
{
    Task<List<Achievement>> GetAchievementsAsync(int userId);
    Task<UserLevel?> GetUserLevelAsync(int userId);
    Task<List<LeaderboardEntry>> GetLeaderboardAsync(LeaderboardScope scope, StatPeriod period, int currentUserId);
    Task AwardXpAsync(int userId, int xp);
    Task CheckAndAwardAchievementsAsync(int userId);
}
