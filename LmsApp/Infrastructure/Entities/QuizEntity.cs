using SQLite;
using System.Text.Json;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Infrastructure.Entities;

[Table("quizzes")]
public class QuizEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CourseId { get; set; }

    [Indexed]
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitSeconds { get; set; }
    public bool IsFinal { get; set; }
    public int MaxAttempts { get; set; } = 3;
    public int PassPercent { get; set; } = 80;

    public Quiz ToDomain() => new()
    {
        Id = Id,
        CourseId = CourseId,
        ModuleId = ModuleId,
        Title = Title,
        TimeLimitSeconds = TimeLimitSeconds,
        IsFinal = IsFinal,
        MaxAttempts = MaxAttempts,
        PassPercent = PassPercent
    };
}

[Table("questions")]
public class QuestionEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Type { get; set; }
    public string OptionsJson { get; set; } = "[]";
    public string CorrectIndicesJson { get; set; } = "[]";
    public string Explanation { get; set; } = string.Empty;

    public Question ToDomain() => new()
    {
        Id = Id,
        QuizId = QuizId,
        Text = Text,
        ImageUrl = ImageUrl,
        Type = (QuestionType)Type,
        Options = JsonSerializer.Deserialize<List<string>>(OptionsJson) ?? new(),
        CorrectOptionIndices = JsonSerializer.Deserialize<List<int>>(CorrectIndicesJson) ?? new(),
        Explanation = Explanation
    };
}

[Table("quiz_results")]
public class QuizResultEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int QuizId { get; set; }

    [Indexed]
    public int UserId { get; set; }
    public int Score { get; set; }
    public int PassedPercent { get; set; }
    public bool Passed { get; set; }
    public int AttemptNumber { get; set; }
    public int TimeSpentSeconds { get; set; }
    public string AnswersJson { get; set; } = "[]";
    public string CompletedAt { get; set; } = DateTime.UtcNow.ToString("O");
}
