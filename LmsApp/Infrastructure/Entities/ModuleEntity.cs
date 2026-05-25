using SQLite;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Infrastructure.Entities;

[Table("modules")]
public class ModuleEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Type { get; set; }
    public int DurationMin { get; set; }
    public int Status { get; set; }
    public int OrderIndex { get; set; }
    public int? LessonId { get; set; }
    public int? QuizId { get; set; }

    public Module ToDomain() => new()
    {
        Id = Id,
        CourseId = CourseId,
        Title = Title,
        Type = (ModuleType)Type,
        DurationMin = DurationMin,
        Status = (ModuleStatus)Status,
        OrderIndex = OrderIndex,
        LessonId = LessonId,
        QuizId = QuizId
    };
}
