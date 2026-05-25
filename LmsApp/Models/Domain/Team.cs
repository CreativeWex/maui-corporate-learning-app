namespace LmsApp.Models.Domain;

public class TeamMember
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public int AssignedCourses { get; set; }
    public int CompletedCourses { get; set; }
    public int OverallProgress { get; set; }
    public bool IsAtRisk { get; set; }
    public DateTime? NearestDeadline { get; set; }
    public DateTime? LastReminderSentAt { get; set; }
}

public class TeamStats
{
    public int TotalMembers { get; set; }
    public int AvgProgress { get; set; }
    public int AssignedCount { get; set; }
    public int OverdueCount { get; set; }
    public int AtRiskCount { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}
