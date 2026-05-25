using SQLite;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Infrastructure.Entities;

[Table("lessons")]
public class LessonEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CourseId { get; set; }

    [Indexed]
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public int DurationMin { get; set; }
    public bool IsCompleted { get; set; }

    public Lesson ToDomain() => new()
    {
        Id = Id,
        CourseId = CourseId,
        ModuleId = ModuleId,
        Title = Title,
        Type = (ModuleType)Type,
        Content = Content,
        DurationMin = DurationMin,
        IsCompleted = IsCompleted
    };
}
