namespace LmsApp.Models.Dto;

public class AssignmentRequest
{
    public int CourseId { get; set; }
    public List<int> RecipientIds { get; set; } = new();
    public DateTime DeadlineDate { get; set; }
    public bool IsMandatory { get; set; }
    public List<int> ReminderDays { get; set; } = new();
    public string? Message { get; set; }
}
