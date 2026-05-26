using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

[QueryProperty(nameof(CourseId), "id")]
public partial class CourseDetailViewModel : BaseViewModel
{
    private readonly ICourseService _courses;
    private readonly ISessionService _session;

    [ObservableProperty] private int _courseId;
    [ObservableProperty] private CourseDetail? _course;
    [ObservableProperty] private ObservableCollection<Module> _modules = new();
    [ObservableProperty] private int _overallProgress;
    [ObservableProperty] private string _ctaText = "Начать";
    [ObservableProperty] private bool _hasDeadlineWarning;
    [ObservableProperty] private string _deadlineText = string.Empty;

    public CourseDetailViewModel(ICourseService courses, ISessionService session, IDialogService dialog) : base(dialog)
    {
        _courses = courses;
        _session = session;
    }

    partial void OnCourseIdChanged(int value) => LoadCommand.Execute(null);

    [RelayCommand]
    async Task LoadAsync()
    {
        if (CourseId <= 0) return;
        await RunSafeAsync(async () =>
        {
            Course = await _courses.GetCourseDetailAsync(CourseId, _session.CurrentUser?.Id ?? 0);
            if (Course == null) return;

            Modules = new ObservableCollection<Module>(Course.Modules);
            OverallProgress = Course.ProgressPercent;

            int completed = Course.Modules.Count(m => m.Status == ModuleStatus.Completed);
            int total = Course.Modules.Count;
            OverallProgress = total > 0 ? (int)Math.Round((double)completed / total * 100) : 0;

            CtaText = OverallProgress == 0 ? "Начать" : OverallProgress == 100 ? "Повторить" : "Продолжить";

            if (Course.DeadlineDate.HasValue)
            {
                var days = (Course.DeadlineDate.Value - DateTime.Today).TotalDays;
                if (days <= 3 && days >= 0)
                {
                    HasDeadlineWarning = true;
                    DeadlineText = days == 0 ? "Дедлайн сегодня!" : $"Дедлайн через {(int)days} дн.";
                }
            }
        });
    }

    [RelayCommand]
    async Task StartContinueAsync()
    {
        if (Course == null) return;
        var firstIncomplete = Course.Modules.FirstOrDefault(m =>
            m.Status != ModuleStatus.Completed && m.Status != ModuleStatus.Locked);
        if (firstIncomplete == null)
            firstIncomplete = Course.Modules.FirstOrDefault();
        if (firstIncomplete == null) return;
        await OpenModule(firstIncomplete);
    }

    [RelayCommand]
    async Task OpenModuleAsync(Module module)
    {
        if (module.Status == ModuleStatus.Locked)
        {
            if (DialogService != null)
                await DialogService.ShowAlertAsync("Заблокировано", "Завершите предыдущий модуль");
            return;
        }
        await OpenModule(module);
    }

    static async Task OpenModule(Module module)
    {
        if (module.Type == ModuleType.Quiz && module.QuizId.HasValue)
            await Shell.Current.GoToAsync($"quiz?id={module.QuizId}");
        else if (module.LessonId.HasValue)
            await Shell.Current.GoToAsync($"lesson?id={module.LessonId}&moduleId={module.Id}&courseId={module.CourseId}");
    }
}
