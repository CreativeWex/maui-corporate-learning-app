using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class AchievementsViewModel : BaseViewModel
{
    private readonly IGamificationService _gamification;
    private readonly ISessionService _session;

    [ObservableProperty] private ObservableCollection<Achievement> _achievements = new();
    [ObservableProperty] private UserLevel? _userLevel;
    [ObservableProperty] private int _unlockedCount;
    [ObservableProperty] private bool _isRefreshing;

    public AchievementsViewModel(IGamificationService gamification, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _gamification = gamification;
        _session = session;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var userId = _session.CurrentUser?.Id ?? 0;
            var (achieveTask, levelTask) = (
                _gamification.GetAchievementsAsync(userId),
                _gamification.GetUserLevelAsync(userId)
            );
            await Task.WhenAll(achieveTask, levelTask);
            var list = await achieveTask;
            Achievements = new ObservableCollection<Achievement>(list);
            UnlockedCount = list.Count(a => a.IsUnlocked);
            UserLevel = await levelTask;
        });
    }

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
        IsRefreshing = false;
    }
}
