using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

[QueryProperty(nameof(QuizId), "id")]
public partial class QuizViewModel : BaseViewModel
{
    private readonly IQuizService _quizService;
    private readonly ISessionService _session;
    private DateTime _quizStart;
    private List<Answer> _answers = new();

    [ObservableProperty] private int _quizId;
    [ObservableProperty] private Quiz? _quiz;
    [ObservableProperty] private Question? _currentQuestion;
    [ObservableProperty] private int _questionIndex;
    [ObservableProperty] private int _totalQuestions;
    [ObservableProperty] private bool _isAnswered;
    [ObservableProperty] private bool _isCorrect;
    [ObservableProperty] private string _explanation = string.Empty;
    [ObservableProperty] private int _attemptsLeft;
    [ObservableProperty] private bool _noAttemptsLeft;
    [ObservableProperty] private List<int> _selectedOptionIndices = new();
    [ObservableProperty] private ObservableCollection<OptionViewModel> _options = new();

    public LessonViewModel? LessonVm { get; set; }

    public QuizViewModel(IQuizService quizService, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _quizService = quizService;
        _session = session;
    }

    partial void OnQuizIdChanged(int value)
    {
        if (value > 0) LoadCommand.Execute(null);
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        if (QuizId <= 0) return;
        await RunSafeAsync(async () =>
        {
            var userId = _session.CurrentUser?.Id ?? 0;
            var quiz = await _quizService.GetQuizAsync(QuizId);
            if (quiz == null) return;

            var left = await _quizService.GetAttemptsLeftAsync(userId, QuizId, quiz.MaxAttempts);
            AttemptsLeft = left;
            NoAttemptsLeft = left <= 0;
            if (NoAttemptsLeft) return;

            Quiz = quiz;
            TotalQuestions = quiz.Questions.Count;
            _answers = new List<Answer>(new Answer[TotalQuestions]);
            _quizStart = DateTime.Now;
            ShowQuestion(0);
        });
    }

    void ShowQuestion(int index)
    {
        if (Quiz == null || index >= Quiz.Questions.Count) return;
        QuestionIndex = index;
        CurrentQuestion = Quiz.Questions[index];
        IsAnswered = false;
        IsCorrect = false;
        Explanation = string.Empty;
        SelectedOptionIndices = new();
        Options = new ObservableCollection<OptionViewModel>(
            CurrentQuestion.Options.Select((opt, i) => new OptionViewModel { Index = i, Text = opt }));
    }

    [RelayCommand]
    void SelectOption(int index)
    {
        if (IsAnswered || CurrentQuestion == null) return;

        if (CurrentQuestion.Type == QuestionType.SingleChoice || CurrentQuestion.Type == QuestionType.TrueFalse)
        {
            SelectedOptionIndices = new List<int> { index };
        }
        else
        {
            var list = new List<int>(SelectedOptionIndices);
            if (list.Contains(index)) list.Remove(index);
            else list.Add(index);
            SelectedOptionIndices = list;
        }

        foreach (var opt in Options)
            opt.IsSelected = SelectedOptionIndices.Contains(opt.Index);
    }

    [RelayCommand]
    void SubmitAnswer()
    {
        if (CurrentQuestion == null || IsAnswered || !SelectedOptionIndices.Any()) return;
        IsAnswered = true;

        var correct = new HashSet<int>(CurrentQuestion.CorrectOptionIndices);
        var selected = new HashSet<int>(SelectedOptionIndices);
        IsCorrect = correct.SetEquals(selected);
        Explanation = CurrentQuestion.Explanation;

        foreach (var opt in Options)
        {
            opt.IsCorrect = correct.Contains(opt.Index);
            opt.IsWrong = selected.Contains(opt.Index) && !correct.Contains(opt.Index);
        }

        _answers[QuestionIndex] = new Answer
        {
            QuestionId = CurrentQuestion.Id,
            SelectedOptionIndices = SelectedOptionIndices,
            IsCorrect = IsCorrect
        };
    }

    [RelayCommand]
    async Task NextQuestionAsync()
    {
        if (CurrentQuestion != null && !IsAnswered)
        {
            _answers[QuestionIndex] = new Answer
            {
                QuestionId = CurrentQuestion.Id,
                SelectedOptionIndices = new(),
                IsSkipped = true
            };
        }

        if (QuestionIndex + 1 < TotalQuestions)
        {
            ShowQuestion(QuestionIndex + 1);
        }
        else
        {
            await FinishQuizAsync();
        }
    }

    [RelayCommand]
    async Task SkipAsync() => await NextQuestionAsync();

    async Task FinishQuizAsync()
    {
        if (Quiz == null) return;
        var elapsed = (int)(DateTime.Now - _quizStart).TotalSeconds;
        var userId = _session.CurrentUser?.Id ?? 0;

        var result = await _quizService.CalculateResultAsync(QuizId, userId, _answers, elapsed);
        await _quizService.SubmitResultAsync(result);

        await Shell.Current.GoToAsync($"quiz-result?passed={result.Passed}&score={result.Score}&total={TotalQuestions}&percent={result.PassedPercent}&quizId={QuizId}&isFinal={Quiz.IsFinal}&courseId={Quiz.CourseId}");
    }

    [RelayCommand]
    static async Task GoBackAsync()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Выйти из теста?", "Прогресс будет потерян", "Выйти", "Остаться");
        if (confirm) await Shell.Current.GoToAsync("..");
    }
}

public partial class OptionViewModel : ObservableObject
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private bool _isCorrect;
    [ObservableProperty] private bool _isWrong;
}
