using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

[QueryProperty(nameof(LessonId), "id")]
[QueryProperty(nameof(ModuleId), "moduleId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class LessonViewModel : BaseViewModel
{
    private readonly ILessonService _lessonService;
    private readonly ISessionService _session;
    private DateTime _sessionStart;

    [ObservableProperty] private int _lessonId;
    [ObservableProperty] private int _moduleId;
    [ObservableProperty] private int _courseId;
    [ObservableProperty] private Lesson? _lesson;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private bool _isArticle;
    [ObservableProperty] private bool _isFlashcards;
    [ObservableProperty] private bool _isInfographic;
    [ObservableProperty] private string _htmlContent = string.Empty;
    [ObservableProperty] private List<FlashCard> _flashCards = new();
    [ObservableProperty] private int _currentCardIndex;
    [ObservableProperty] private bool _isCardFlipped;
    [ObservableProperty] private FlashCard? _currentCard;

    public LessonViewModel(ILessonService lessonService, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _lessonService = lessonService;
        _session = session;
    }

    partial void OnLessonIdChanged(int value)
    {
        if (value > 0) LoadCommand.Execute(null);
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        if (LessonId <= 0) return;
        await RunSafeAsync(async () =>
        {
            _sessionStart = DateTime.Now;
            Lesson = await _lessonService.GetLessonAsync(LessonId);
            if (Lesson == null) return;

            IsCompleted = Lesson.IsCompleted;
            IsArticle = Lesson.Type == ModuleType.Article;
            IsFlashcards = Lesson.Type == ModuleType.Flashcards;
            IsInfographic = Lesson.Type == ModuleType.Infographic;

            if (IsArticle || IsInfographic)
                HtmlContent = BuildHtml(Lesson.Content);

            if (IsFlashcards)
            {
                try
                {
                    var cards = System.Text.Json.JsonSerializer.Deserialize<List<FlashCardRaw>>(Lesson.Content);
                    FlashCards = cards?.Select(c => new FlashCard { Front = c.Front, Back = c.Back }).ToList() ?? new();
                    CurrentCard = FlashCards.FirstOrDefault();
                }
                catch { FlashCards = new(); }
            }
        });
    }

    [RelayCommand]
    async Task MarkCompletedAsync()
    {
        if (IsCompleted || LessonId <= 0) return;
        await RunSafeAsync(async () =>
        {
            IsCompleted = true;
            await _lessonService.MarkCompletedAsync(LessonId, ModuleId, CourseId, _session.CurrentUser?.Id ?? 0);
            await SaveSession();
            if (DialogService != null)
                await DialogService.ShowToastAsync("+10 XP за урок!");
        });
    }

    [RelayCommand]
    void FlipCard()
    {
        IsCardFlipped = !IsCardFlipped;
        if (CurrentCard != null) CurrentCard.IsFlipped = IsCardFlipped;
    }

    [RelayCommand]
    void NextCard()
    {
        if (CurrentCardIndex < FlashCards.Count - 1)
        {
            CurrentCardIndex++;
            CurrentCard = FlashCards[CurrentCardIndex];
            IsCardFlipped = false;
        }
        else if (!IsCompleted)
        {
            MarkCompletedCommand.Execute(null);
        }
    }

    [RelayCommand]
    void PreviousCard()
    {
        if (CurrentCardIndex > 0)
        {
            CurrentCardIndex--;
            CurrentCard = FlashCards[CurrentCardIndex];
            IsCardFlipped = false;
        }
    }

    [RelayCommand]
    static Task GoBack() => Shell.Current.GoToAsync("..");

    async Task SaveSession()
    {
        var elapsed = (int)(DateTime.Now - _sessionStart).TotalSeconds;
        await _lessonService.SaveSessionAsync(new LearningSession
        {
            UserId = _session.CurrentUser?.Id ?? 0,
            LessonId = LessonId,
            StartedAt = _sessionStart,
            CompletedAt = DateTime.Now,
            TimeSpentSec = elapsed
        });
    }

    static string BuildHtml(string markdown)
    {
        try
        {
            var html = Markdig.Markdown.ToHtml(markdown);
            return $$"""
                <!DOCTYPE html>
                <html><head>
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <style>
                  body { font-family: -apple-system, sans-serif; font-size: 16px; padding: 16px; color: #374151; line-height: 1.6; }
                  h2 { color: #4F46E5; font-size: 22px; margin-top: 24px; }
                  h3 { color: #111827; font-size: 18px; }
                  code { background: #F3F4F6; padding: 2px 6px; border-radius: 4px; font-size: 14px; }
                  pre { background: #1F2937; color: #E5E7EB; padding: 16px; border-radius: 8px; overflow-x: auto; }
                  pre code { background: none; color: inherit; }
                  ul, ol { padding-left: 20px; }
                  li { margin: 4px 0; }
                  strong { color: #111827; }
                </style></head><body>{{html}}</body></html>
                """;
        }
        catch
        {
            return $"<html><body><p>{markdown}</p></body></html>";
        }
    }
}

public class FlashCard
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public bool IsFlipped { get; set; }
}

file class FlashCardRaw
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
}
