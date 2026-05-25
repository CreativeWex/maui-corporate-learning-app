using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class TeamDashboardViewModel : BaseViewModel
{
    private readonly ITeamService _team;
    private readonly ISessionService _session;

    [ObservableProperty] private TeamStats _teamStats = new();
    [ObservableProperty] private ObservableCollection<TeamMember> _members = new();
    [ObservableProperty] private ObservableCollection<TeamMember> _atRisk = new();
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private bool _hasAtRisk;

    public TeamDashboardViewModel(ITeamService team, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _team = team;
        _session = session;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var managerId = _session.CurrentUser?.Id ?? 0;
            var (statsTask, membersTask, atRiskTask) = (
                _team.GetTeamStatsAsync(managerId),
                _team.GetTeamMembersAsync(managerId),
                _team.GetAtRiskMembersAsync(managerId)
            );
            await Task.WhenAll(statsTask, membersTask, atRiskTask);

            TeamStats = await statsTask;
            Members = new ObservableCollection<TeamMember>(await membersTask);
            var risk = await atRiskTask;
            AtRisk = new ObservableCollection<TeamMember>(risk);
            HasAtRisk = risk.Any();
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
    static Task AssignCourseAsync() => Shell.Current.GoToAsync("assign-course");
}
