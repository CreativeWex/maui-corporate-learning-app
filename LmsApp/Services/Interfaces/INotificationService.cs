namespace LmsApp.Services.Interfaces;

public interface INotificationService
{
    Task SendReminderAsync(int userId, string message);
    Task NotifyAssignmentAsync(int userId, string courseName);
    bool CanSendReminder(int memberId);
    void RecordReminderSent(int memberId);
}
