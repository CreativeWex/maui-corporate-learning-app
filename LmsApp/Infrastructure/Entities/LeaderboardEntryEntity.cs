using SQLite;
using LmsApp.Models.Domain;

namespace LmsApp.Infrastructure.Entities;

[Table("leaderboard")]
public class LeaderboardEntryEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Department { get; set; } = string.Empty;
    public int TotalXp { get; set; }
    public int BadgeCount { get; set; }

    public LeaderboardEntry ToDomain(int rank, int currentUserId) => new()
    {
        Rank = rank,
        UserId = UserId,
        DisplayName = DisplayName,
        AvatarUrl = AvatarUrl,
        Department = Department,
        TotalXp = TotalXp,
        BadgeCount = BadgeCount,
        IsCurrentUser = UserId == currentUserId
    };
}
