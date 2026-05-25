using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly ISessionService _session;
    private readonly ISettingsService _settings;

    [ObservableProperty] private User? _currentUser;
    [ObservableProperty] private AppSettings _appSettings = new();
    [ObservableProperty] private bool _isDarkTheme;
    [ObservableProperty] private string _selectedTheme = "light";

    public ProfileViewModel(IAuthService auth, ISessionService session,
        ISettingsService settings, IDialogService dialog)
        : base(dialog)
    {
        _auth = auth;
        _session = session;
        _settings = settings;
    }

    [RelayCommand]
    Task LoadAsync()
    {
        CurrentUser = _session.CurrentUser;
        AppSettings = _settings.GetSettings();
        SelectedTheme = AppSettings.Theme;
        IsDarkTheme = AppSettings.Theme == "dark";
        return Task.CompletedTask;
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        var theme = value ? "dark" : "light";
        SelectedTheme = theme;
        _settings.SetTheme(theme);
        Application.Current!.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
    }

    [RelayCommand]
    void ToggleNotifyAssignments()
    {
        AppSettings.NotifyAssignments = !AppSettings.NotifyAssignments;
        _settings.SetNotificationPref("assignments", AppSettings.NotifyAssignments);
    }

    [RelayCommand]
    void ToggleNotifyDeadlines()
    {
        AppSettings.NotifyDeadlines = !AppSettings.NotifyDeadlines;
        _settings.SetNotificationPref("deadlines", AppSettings.NotifyDeadlines);
    }

    [RelayCommand]
    async Task LogoutAsync()
    {
        bool confirm = DialogService == null || await DialogService.ShowConfirmAsync("Выйти?", "Вы уверены, что хотите выйти?");
        if (!confirm) return;
        await _auth.LogoutAsync();
        await Shell.Current.GoToAsync("//splash");
    }
}
