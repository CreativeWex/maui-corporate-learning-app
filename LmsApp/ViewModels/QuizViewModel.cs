using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Helpers;
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(QuestionNumber))]
    [NotifyPropertyChangedFor(nameof(QuestionProgressFraction))]
    private int _questionIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(QuestionProgressFraction))]
    private int _totalQuestions;

    public int QuestionNumber => QuestionIndex + 1;
    public double QuestionProgressFraction => TotalQuestions > 0 ? (double)(QuestionIndex + 1) / TotalQuestions : 0;

    [ObservableProperty] private bool _isAnswered;
    [ObservableProperty] private bool _isCorrect;
    [ObservableProperty] private string _explanation = string.Empty;
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
            var quiz = await _quizService.GetQuizAsync(QuizId);
            if (quiz == null) return;

            if (quiz.Questions.Count == 0)
            {
                if (DialogService != null)
                    await DialogService.ShowAlertAsync("Тест недоступен", "В этом тесте пока нет вопросов");
                await Nav.GoBackAsync();
                return;
            }

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
        var isMulti = CurrentQuestion.Type == QuestionType.MultipleChoice;
        Options = new ObservableCollection<OptionViewModel>(
            CurrentQuestion.Options.Select((opt, i) => new OptionViewModel { Index = i, Text = opt, IsMultiChoice = isMulti }));
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
    static async Task ExitQuizAsync()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Выйти из теста?", "Прогресс будет потерян", "Выйти", "Остаться");
        if (confirm) await Nav.GoBackAsync();
    }
}

public partial class OptionViewModel : ObservableObject
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsMultiChoice { get; set; }
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private bool _isCorrect;
    [ObservableProperty] private bool _isWrong;
}
