using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

[QueryProperty(nameof(Passed), "passed")]
[QueryProperty(nameof(Score), "score")]
[QueryProperty(nameof(Total), "total")]
[QueryProperty(nameof(Percent), "percent")]
[QueryProperty(nameof(QuizId), "quizId")]
[QueryProperty(nameof(IsFinal), "isFinal")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class QuizResultViewModel : BaseViewModel
{
    [ObservableProperty] private bool _passed;
    [ObservableProperty] private int _score;
    [ObservableProperty] private int _total;
    [ObservableProperty] private int _percent;
    [ObservableProperty] private int _quizId;
    [ObservableProperty] private bool _isFinal;
    [ObservableProperty] private int _courseId;

    [ObservableProperty] private string _resultTitle = string.Empty;
    [ObservableProperty] private string _resultSubtitle = string.Empty;
    [ObservableProperty] private string _resultEmoji = string.Empty;
    [ObservableProperty] private bool _showCertificate;

    public QuizResultViewModel(IDialogService dialog) : base(dialog) { }

    partial void OnPassedChanged(bool value) => UpdateResultText();
    partial void OnPercentChanged(int value) => UpdateResultText();
    partial void OnIsFinalChanged(bool value) => UpdateResultText();

    void UpdateResultText()
    {
        if (Passed)
        {
            ResultEmoji = "🎉";
            ResultTitle = "Тест пройден!";
            ResultSubtitle = $"Вы набрали {Percent}% правильных ответов";
            ShowCertificate = IsFinal;
        }
        else
        {
            ResultEmoji = "😔";
            ResultTitle = "Не пройден";
            ResultSubtitle = $"Вы набрали {Percent}%. Для прохождения нужно 80%";
            ShowCertificate = false;
        }
    }

    [RelayCommand]
    static Task GoHomAsync() => Shell.Current.Navigation.PopToRootAsync();

    [RelayCommand]
    Task RetryAsync() => Shell.Current.GoToAsync($"quiz?id={QuizId}");

    [RelayCommand]
    Task GoToCourseAsync() => Shell.Current.GoToAsync($"course-detail?id={CourseId}");

    [RelayCommand]
    Task OpenCertificatesAsync() => Shell.Current.GoToAsync("certificates");
}
