using SQLite;
using System.Text.Json;
using LmsApp.Models.Domain;

namespace LmsApp.Infrastructure.Entities;

[Table("assignments")]
public class AssignmentEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CourseId { get; set; }

    [Indexed]
    public int UserId { get; set; }
    public int AssignedById { get; set; }
    public string DeadlineDate { get; set; } = string.Empty;
    public string AssignedAt { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public string? Message { get; set; }
    public string ReminderDaysJson { get; set; } = "[]";

    public Assignment ToDomain() => new()
    {
        Id = Id,
        CourseId = CourseId,
        UserId = UserId,
        AssignedById = AssignedById,
        DeadlineDate = DateTime.Parse(DeadlineDate),
        AssignedAt = DateTime.Parse(AssignedAt),
        IsMandatory = IsMandatory,
        Message = Message,
        ReminderDays = JsonSerializer.Deserialize<List<int>>(ReminderDaysJson) ?? new()
    };
}
