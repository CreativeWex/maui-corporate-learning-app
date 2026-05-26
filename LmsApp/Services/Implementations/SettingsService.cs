using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class SettingsService : ISettingsService
{
    public AppSettings GetSettings() => new()
    {
        Language = Preferences.Get("lang", "ru"),
        Theme = Preferences.Get("theme", "light"),
        NotifyAssignments = Preferences.Get("notify_assignments", true),
        NotifyDeadlines = Preferences.Get("notify_deadlines", true),
        NotifyAchievements = Preferences.Get("notify_achievements", true),
        NotifyStreak = Preferences.Get("notify_streak", true)
    };

    public void SetTheme(string theme)
    {
        Preferences.Set("theme", theme);
        Application.Current!.UserAppTheme = theme == "dark" ? AppTheme.Dark : AppTheme.Light;
    }

    public void SetNotificationPref(string key, bool value)
        => Preferences.Set(key, value);

    public void SetLanguage(string lang)
        => Preferences.Set("lang", lang);
}
