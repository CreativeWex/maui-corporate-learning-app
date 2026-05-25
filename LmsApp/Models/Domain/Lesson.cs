using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class Lesson
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ModuleType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public int DurationMin { get; set; }
    public bool IsCompleted { get; set; }
}
