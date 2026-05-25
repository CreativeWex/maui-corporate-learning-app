using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public bool IsAssigned { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public bool IsNew { get; set; }
}

public class CourseDetail : Course
{
    public string Description { get; set; } = string.Empty;
    public List<Module> Modules { get; set; } = new();
}
