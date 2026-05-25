using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class Module
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ModuleType Type { get; set; }
    public int DurationMin { get; set; }
    public ModuleStatus Status { get; set; }
    public int OrderIndex { get; set; }
    public int? LessonId { get; set; }
    public int? QuizId { get; set; }
}
