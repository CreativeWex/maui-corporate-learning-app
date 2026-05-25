using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class LeaderboardViewModel : BaseViewModel
{
    private readonly IGamificationService _gamification;
    private readonly ISessionService _session;

    [ObservableProperty] private ObservableCollection<LeaderboardEntry> _entries = new();
    [ObservableProperty] private LeaderboardEntry? _first;
    [ObservableProperty] private LeaderboardEntry? _second;
    [ObservableProperty] private LeaderboardEntry? _third;
    [ObservableProperty] private ObservableCollection<LeaderboardEntry> _rest = new();
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _selectedScope = "Компания";

    public static List<string> Scopes { get; } = new() { "Команда", "Компания" };

    public LeaderboardViewModel(IGamificationService gamification, ISessionService session, IDialogService dialog)
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
            var scope = SelectedScope == "Команда" ? LeaderboardScope.Team : LeaderboardScope.Company;
            var list = await _gamification.GetLeaderboardAsync(scope, StatPeriod.AllTime, userId);
            Entries = new ObservableCollection<LeaderboardEntry>(list);
            First  = list.ElementAtOrDefault(0);
            Second = list.ElementAtOrDefault(1);
            Third  = list.ElementAtOrDefault(2);
            Rest   = new ObservableCollection<LeaderboardEntry>(list.Skip(3));
        });
    }

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    async Task SelectScopeAsync(string scope)
    {
        SelectedScope = scope;
        await LoadAsync();
    }
}
