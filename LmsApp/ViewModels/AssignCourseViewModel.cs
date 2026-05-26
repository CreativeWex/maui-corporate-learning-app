using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Helpers;
using LmsApp.Models.Domain;
using LmsApp.Models.Dto;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class AssignCourseViewModel : BaseViewModel
{
    private readonly ICourseService _courses;
    private readonly ITeamService _team;
    private readonly IAssignmentService _assignments;
    private readonly ISessionService _session;

    // Step 1: Select course
    [ObservableProperty] private ObservableCollection<SelectableCourse> _availableCourses = new();
    [ObservableProperty] private Course? _selectedCourse;

    // Step 2: Select recipients
    [ObservableProperty] private ObservableCollection<SelectableTeamMember> _members = new();

    // Step 3: Configure
    [ObservableProperty] private DateTime _deadline = DateTime.Today.AddDays(14);
    [ObservableProperty] private bool _isMandatory = true;
    [ObservableProperty] private string _message = string.Empty;

    // Wizard state
    [ObservableProperty] private int _currentStep = 1;
    [ObservableProperty] private bool _isStep1 = true;
    [ObservableProperty] private bool _isStep2;
    [ObservableProperty] private bool _isStep3;
    [ObservableProperty] private bool _canGoNext;

    public AssignCourseViewModel(ICourseService courses, ITeamService team,
        IAssignmentService assignments, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _courses = courses;
        _team = team;
        _assignments = assignments;
        _session = session;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var userId = _session.CurrentUser?.Id ?? 0;
            var (courseTask, memberTask) = (
                _courses.GetCatalogAsync(0),
                _team.GetTeamMembersAsync(userId)
            );
            await Task.WhenAll(courseTask, memberTask);

            AvailableCourses = new ObservableCollection<SelectableCourse>((await courseTask).Select(c => new SelectableCourse(c)));
            Members = new ObservableCollection<SelectableTeamMember>(
                (await memberTask).Select(m => new SelectableTeamMember(m)));
            UpdateCanGoNext();
        });
    }

    partial void OnSelectedCourseChanged(Course? value) => UpdateCanGoNext();
    partial void OnCurrentStepChanged(int value)
    {
        IsStep1 = value == 1;
        IsStep2 = value == 2;
        IsStep3 = value == 3;
        UpdateCanGoNext();
    }

    void UpdateCanGoNext()
    {
        CanGoNext = CurrentStep switch
        {
            1 => SelectedCourse != null,
            2 => Members.Any(m => m.IsSelected),
            _ => true
        };
    }

    [RelayCommand]
    void SelectCourse(SelectableCourse course)
    {
        foreach (var c in AvailableCourses) c.IsSelected = false;
        course.IsSelected = true;
        SelectedCourse = course.Course;
        UpdateCanGoNext();
    }

    [RelayCommand]
    void ToggleMember(SelectableTeamMember member)
    {
        member.IsSelected = !member.IsSelected;
        UpdateCanGoNext();
    }

    [RelayCommand]
    void NextStep()
    {
        if (CurrentStep < 3) CurrentStep++;
    }

    [RelayCommand]
    void PrevStep()
    {
        if (CurrentStep > 1) CurrentStep--;
    }

    [RelayCommand]
    async Task SubmitAsync()
    {
        if (SelectedCourse == null) return;
        await RunSafeAsync(async () =>
        {
            var assignerId = _session.CurrentUser?.Id ?? 0;
            var recipientIds = Members.Where(m => m.IsSelected).Select(m => m.UserId).ToList();

            var request = new AssignmentRequest
            {
                CourseId = SelectedCourse.Id,
                RecipientIds = recipientIds,
                DeadlineDate = Deadline,
                IsMandatory = IsMandatory,
                Message = string.IsNullOrWhiteSpace(Message) ? null : Message,
                ReminderDays = new List<int> { 3, 1 }
            };

            await _assignments.AssignAsync(request, assignerId);

            if (DialogService != null)
                await DialogService.ShowToastAsync($"Курс назначен {recipientIds.Count} сотрудникам");

            ResetForm();
            await Nav.GoBackAsync();
        });
    }

    [RelayCommand]
    async Task CancelAsync()
    {
        ResetForm();
        await Nav.GoBackAsync();
    }

    void ResetForm()
    {
        foreach (var c in AvailableCourses) c.IsSelected = false;
        foreach (var m in Members) m.IsSelected = false;
        SelectedCourse = null;
        Message = string.Empty;
        Deadline = DateTime.Today.AddDays(14);
        IsMandatory = true;
        CurrentStep = 1;
    }
}

public partial class SelectableCourse : ObservableObject
{
    public Course Course { get; }
    public int Id => Course.Id;
    public string Title => Course.Title;
    public string Category => Course.Category;
    public int DurationMinutes => Course.DurationMinutes;
    [ObservableProperty] private bool _isSelected;

    public SelectableCourse(Course course) => Course = course;
}

public partial class SelectableTeamMember : ObservableObject
{
    public int UserId { get; }
    public string Name { get; }
    public string Position { get; }
    [ObservableProperty] private bool _isSelected;

    public SelectableTeamMember(TeamMember member)
    {
        UserId = member.UserId;
        Name = member.Name;
        Position = member.Position;
    }
}
