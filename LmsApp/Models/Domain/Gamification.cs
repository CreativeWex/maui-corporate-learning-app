using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class UserLevel
{
    public int Level { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int CurrentXp { get; set; }
    public int XpToNext { get; set; }
    public double ProgressPercent => XpToNext > 0 ? (double)CurrentXp / XpToNext * 100 : 100;
}

public class LeaderboardEntry
{
    public int Rank { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Department { get; set; } = string.Empty;
    public int TotalXp { get; set; }
    public int BadgeCount { get; set; }
    public bool IsCurrentUser { get; set; }
}
