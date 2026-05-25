namespace LmsApp.Models.Domain;

public class Achievement
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int XpReward { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public string Condition { get; set; } = string.Empty;
    public bool IsUnlocked => UnlockedAt.HasValue;
    public double Progress { get; set; }
}
