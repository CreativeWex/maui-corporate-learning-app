using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Services.Interfaces;

public interface IProgressService
{
    Task<ProgressStats> GetStatsAsync(int userId, StatPeriod period);
    Task<List<DailyActivity>> GetDailyActivityAsync(int userId, StatPeriod period);
    Task<int> GetCurrentStreakAsync(int userId);
    Task<UserStats> GetUserStatsAsync(int userId);
}
