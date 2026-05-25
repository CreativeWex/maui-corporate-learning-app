using SQLite;
using LmsApp.Models.Domain;

namespace LmsApp.Infrastructure.Entities;

[Table("learning_sessions")]
public class LearningSessionEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UserId { get; set; }

    [Indexed]
    public int LessonId { get; set; }
    public string StartedAt { get; set; } = string.Empty;
    public string? CompletedAt { get; set; }
    public int TimeSpentSec { get; set; }

    public LearningSession ToDomain() => new()
    {
        Id = Id,
        UserId = UserId,
        LessonId = LessonId,
        StartedAt = DateTime.Parse(StartedAt),
        CompletedAt = CompletedAt != null ? DateTime.Parse(CompletedAt) : null,
        TimeSpentSec = TimeSpentSec
    };
}
