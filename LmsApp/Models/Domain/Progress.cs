using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class LearningSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LessonId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentSec { get; set; }
}

public class ProgressStats
{
    public StatPeriod Period { get; set; }
    public int CompletedCourses { get; set; }
    public double TotalHours { get; set; }
    public int CurrentStreak { get; set; }
    public int MaxStreak { get; set; }
    public int Rank { get; set; }
}

public class DailyActivity
{
    public DateTime Date { get; set; }
    public int MinutesSpent { get; set; }
    public bool HasActivity => MinutesSpent > 0;
}

public class UserStats
{
    public int CompletedCourses { get; set; }
    public double TotalHours { get; set; }
    public int Badges { get; set; }
    public int LeaderboardRank { get; set; }
    public int CurrentStreak { get; set; }
    public int TotalXp { get; set; }
    public int PassedQuizzes { get; set; }
    public int TotalMinutes { get; set; }
}
