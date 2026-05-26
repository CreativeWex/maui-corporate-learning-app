using LmsApp.Models.Domain;

namespace LmsApp.Services.Interfaces;

public interface ISettingsService
{
    AppSettings GetSettings();
    void SetTheme(string theme);
    void SetNotificationPref(string key, bool value);
    void SetLanguage(string lang);
}
