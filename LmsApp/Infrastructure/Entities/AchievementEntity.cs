using SQLite;
using LmsApp.Models.Domain;

namespace LmsApp.Infrastructure.Entities;

[Table("achievements")]
public class AchievementEntity
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int XpReward { get; set; }
    public string Condition { get; set; } = string.Empty;
    public double Progress { get; set; }
}

[Table("user_achievements")]
public class UserAchievementEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UserId { get; set; }

    [Indexed]
    public int AchievementId { get; set; }
    public string UnlockedAt { get; set; } = string.Empty;
}
