using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class CatalogViewModel : BaseViewModel
{
    private readonly ICourseService _courses;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private ObservableCollection<Course> _courses2 = new();

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _selectedFilter = "Все";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleLabel))]
    private bool _isGridView = true;

    public bool IsEmpty => !Courses2.Any() && !IsBusy;
    public string ToggleLabel => IsGridView ? "Список" : "Сетка";

    public static List<string> Filters { get; } = new()
        { "Все", "Назначенные", "IT", "Soft Skills", "Compliance", "Менеджмент" };

    public CatalogViewModel(ICourseService courses, IDialogService dialog) : base(dialog)
    {
        _courses = courses;
    }

    [RelayCommand]
    async Task LoadAsync() => await SearchAsync();

    [RelayCommand]
    async Task SearchAsync()
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        await Task.Delay(300, token).ContinueWith(async t =>
        {
            if (t.IsCanceled) return;
            await RunSafeAsync(async () =>
            {
                var filter = SelectedFilter == "Назначенные" ? null : SelectedFilter;
                var results = await _courses.GetCatalogAsync(filter, SearchText);

                if (SelectedFilter == "Назначенные")
                    results = results.Where(c => c.IsAssigned).ToList();

                Courses2 = new ObservableCollection<Course>(results);
                OnPropertyChanged(nameof(IsEmpty));
            });
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    [RelayCommand]
    async Task SelectFilter(string filter)
    {
        SelectedFilter = filter;
        await SearchAsync();
    }

    [RelayCommand]
    void ToggleView() => IsGridView = !IsGridView;

    [RelayCommand]
    static Task OpenCourse(int courseId)
        => Shell.Current.GoToAsync($"course-detail?id={courseId}");
}
