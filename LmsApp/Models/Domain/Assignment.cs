namespace LmsApp.Models.Domain;

public class Assignment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int UserId { get; set; }
    public int AssignedById { get; set; }
    public DateTime DeadlineDate { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsMandatory { get; set; }
    public string? Message { get; set; }
    public List<int> ReminderDays { get; set; } = new();
}
