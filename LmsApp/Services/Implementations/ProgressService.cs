using LmsApp.Infrastructure;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class ProgressService : IProgressService
{
    private readonly ILocalRepository _repo;

    public ProgressService(ILocalRepository repo) => _repo = repo;

    public async Task<ProgressStats> GetStatsAsync(int userId, StatPeriod period)
    {
        var (from, to) = PeriodRange(period);
        var sessions = await _repo.GetSessionsByUserAsync(userId, from, to);
        var allSessions = await _repo.GetSessionsByUserAsync(userId);

        double hours = sessions.Sum(s => s.TimeSpentSec) / 3600.0;
        int streak = CalculateStreak(allSessions);

        return new ProgressStats
        {
            Period = period,
            CompletedCourses = await CountCompletedCourses(userId),
            TotalHours = Math.Round(hours, 1),
            CurrentStreak = streak,
            MaxStreak = CalculateMaxStreak(allSessions),
            Rank = 6
        };
    }

    public async Task<List<DailyActivity>> GetDailyActivityAsync(int userId, StatPeriod period)
    {
        var (from, to) = PeriodRange(period);
        var sessions = await _repo.GetSessionsByUserAsync(userId, from, to);

        var byDay = sessions
            .GroupBy(s => DateTime.Parse(s.StartedAt).Date)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.TimeSpentSec) / 60);

        var result = new List<DailyActivity>();
        for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
        {
            result.Add(new DailyActivity
            {
                Date = d,
                MinutesSpent = byDay.GetValueOrDefault(d, 0)
            });
        }
        return result;
    }

    public async Task<int> GetCurrentStreakAsync(int userId)
    {
        var sessions = await _repo.GetSessionsByUserAsync(userId);
        return CalculateStreak(sessions);
    }

    public async Task<UserStats> GetUserStatsAsync(int userId)
    {
        var stats = await GetStatsAsync(userId, StatPeriod.AllTime);
        var sessions = await _repo.GetSessionsByUserAsync(userId);
        var totalMinutes = sessions.Sum(s => s.TimeSpentSec / 60);
        var quizResults = await _repo.GetAllQuizResultsByUserAsync(userId);
        var userEntity = await _repo.GetUserByIdAsync(userId);
        return new UserStats
        {
            CompletedCourses = stats.CompletedCourses,
            TotalHours = stats.TotalHours,
            Badges = 3,
            LeaderboardRank = stats.Rank,
            CurrentStreak = CalculateStreak(sessions),
            TotalXp = userEntity?.TotalXp ?? 0,
            PassedQuizzes = quizResults.Count(r => r.Passed),
            TotalMinutes = totalMinutes
        };
    }

    static int CalculateStreak(List<LearningSessionEntity> sessions)
    {
        if (!sessions.Any()) return 0;
        var activeDays = sessions
            .Select(s => DateTime.Parse(s.StartedAt).Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        var today = DateTime.Today;
        int streak = 0;
        var expected = activeDays.Contains(today) ? today : today.AddDays(-1);

        foreach (var day in activeDays)
        {
            if (day == expected) { streak++; expected = expected.AddDays(-1); }
            else if (day < expected) break;
        }
        return streak;
    }

    static int CalculateMaxStreak(List<LearningSessionEntity> sessions)
    {
        if (!sessions.Any()) return 0;
        var days = sessions
            .Select(s => DateTime.Parse(s.StartedAt).Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        int max = 1, current = 1;
        for (int i = 1; i < days.Count; i++)
        {
            if ((days[i] - days[i - 1]).Days == 1) { current++; max = Math.Max(max, current); }
            else current = 1;
        }
        return max;
    }

    async Task<int> CountCompletedCourses(int userId)
    {
        var results = await _repo.GetSessionsByUserAsync(userId);
        return 1; // simplified: will be refined with quiz results in later stage
    }

    static (DateTime from, DateTime to) PeriodRange(StatPeriod period)
    {
        var now = DateTime.Now;
        return period switch
        {
            StatPeriod.Week    => (now.AddDays(-7), now),
            StatPeriod.Month   => (now.AddDays(-30), now),
            StatPeriod.Quarter => (now.AddDays(-90), now),
            _                  => (DateTime.MinValue, now)
        };
    }
}
