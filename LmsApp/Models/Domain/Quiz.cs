using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class Quiz
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitSeconds { get; set; }
    public bool IsFinal { get; set; }
    public int MaxAttempts { get; set; } = 3;
    public int PassPercent { get; set; } = 80;
    public List<Question> Questions { get; set; } = new();
}

public class Question
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public QuestionType Type { get; set; }
    public List<string> Options { get; set; } = new();
    public List<int> CorrectOptionIndices { get; set; } = new();
    public string Explanation { get; set; } = string.Empty;
}

public class Answer
{
    public int QuestionId { get; set; }
    public List<int> SelectedOptionIndices { get; set; } = new();
    public bool IsCorrect { get; set; }
    public bool IsSkipped { get; set; }
}

public class QuizResult
{
    public int QuizId { get; set; }
    public int UserId { get; set; }
    public int Score { get; set; }
    public int PassedPercent { get; set; }
    public bool Passed { get; set; }
    public int AttemptNumber { get; set; }
    public int TimeSpentSeconds { get; set; }
    public List<Answer> AnswerDetails { get; set; } = new();
    public DateTime CompletedAt { get; set; }
}
