namespace LmsApp.Models.Domain;

public class AppSettings
{
    public string Language { get; set; } = "ru";
    public string Theme { get; set; } = "light";
    public bool NotifyAssignments { get; set; } = true;
    public bool NotifyDeadlines { get; set; } = true;
    public bool NotifyAchievements { get; set; } = true;
    public bool NotifyStreak { get; set; } = true;
    public bool IsAnonymousInLeaderboard { get; set; }
}
