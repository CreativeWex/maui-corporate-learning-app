using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class MyProgressViewModel : BaseViewModel
{
    private readonly IProgressService _progress;
    private readonly ISessionService _session;

    [ObservableProperty] private UserStats _stats = new();
    [ObservableProperty] private int _currentStreak;
    [ObservableProperty] private int _completedCourses;
    [ObservableProperty] private double _totalHours;
    [ObservableProperty] private int _passedQuizzes;
    [ObservableProperty] private string _selectedPeriod = "Месяц";
    [ObservableProperty] private ObservableCollection<DailyActivity> _activity = new();
    [ObservableProperty] private bool _isRefreshing;

    public static List<string> Periods { get; } = new() { "Неделя", "Месяц", "Квартал", "Всё время" };

    public MyProgressViewModel(IProgressService progress, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _progress = progress;
        _session = session;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var userId = _session.CurrentUser?.Id ?? 0;
            var period = SelectedPeriod switch
            {
                "Неделя"    => StatPeriod.Week,
                "Квартал"   => StatPeriod.Quarter,
                "Всё время" => StatPeriod.AllTime,
                _           => StatPeriod.Month
            };

            var (statsTask, activityTask, streakTask) = (
                _progress.GetUserStatsAsync(userId),
                _progress.GetDailyActivityAsync(userId, period),
                _progress.GetCurrentStreakAsync(userId)
            );

            await Task.WhenAll(statsTask, activityTask, streakTask);

            Stats = await statsTask;
            CompletedCourses = Stats.CompletedCourses;
            TotalHours = Stats.TotalHours;
            PassedQuizzes = Stats.PassedQuizzes;
            CurrentStreak = await streakTask;
            Activity = new ObservableCollection<DailyActivity>(await activityTask);
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
    async Task SelectPeriodAsync(string period)
    {
        SelectedPeriod = period;
        await LoadAsync();
    }
}
