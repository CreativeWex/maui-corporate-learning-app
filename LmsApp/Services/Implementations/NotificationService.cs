using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly Dictionary<int, DateTime> _lastReminders = new();
    private static readonly TimeSpan ThrottleInterval = TimeSpan.FromHours(24);

    public Task SendReminderAsync(int userId, string message)
    {
        _lastReminders[userId] = DateTime.Now;
        return Task.CompletedTask;
    }

    public Task NotifyAssignmentAsync(int userId, string courseName)
        => Task.CompletedTask;

    public bool CanSendReminder(int memberId)
        => !_lastReminders.TryGetValue(memberId, out var last) ||
           DateTime.Now - last >= ThrottleInterval;

    public void RecordReminderSent(int memberId)
        => _lastReminders[memberId] = DateTime.Now;
}
