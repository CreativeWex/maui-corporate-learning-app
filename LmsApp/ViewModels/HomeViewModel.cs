using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly ICourseService _courses;
    private readonly IProgressService _progress;
    private readonly ISessionService _session;

    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private int _streakDays;
    [ObservableProperty] private ObservableCollection<Course> _continueLearning = new();
    [ObservableProperty] private ObservableCollection<Course> _newAssignments = new();
    [ObservableProperty] private UserStats _stats = new();
    [ObservableProperty] private bool _isRefreshing;

    public HomeViewModel(ICourseService courses, IProgressService progress, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _courses = courses;
        _progress = progress;
        _session = session;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var user = _session.CurrentUser;
            if (user == null) return;

            var hour = DateTime.Now.Hour;
            Greeting = hour switch
            {
                < 12 => $"Доброе утро, {user.Name.Split(' ')[0]}!",
                < 18 => $"Добрый день, {user.Name.Split(' ')[0]}!",
                _    => $"Добрый вечер, {user.Name.Split(' ')[0]}!"
            };

            var (continueTask, newTask, statsTask, streakTask) = (
                _courses.GetContinueLearningAsync(user.Id),
                _courses.GetNewAssignmentsAsync(user.Id),
                _progress.GetUserStatsAsync(user.Id),
                _progress.GetCurrentStreakAsync(user.Id)
            );

            await Task.WhenAll(continueTask, newTask, statsTask, streakTask);

            ContinueLearning = new ObservableCollection<Course>(await continueTask);
            NewAssignments   = new ObservableCollection<Course>(await newTask);
            Stats            = await statsTask;
            StreakDays       = await streakTask;
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
    static Task OpenCourse(int courseId)
        => Shell.Current.GoToAsync($"course-detail?id={courseId}");
}
