using SQLite;
using LmsApp.Models.Domain;

namespace LmsApp.Infrastructure.Entities;

[Table("courses")]
public class CourseEntity
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsNew { get; set; }

    public Course ToDomain() => new()
    {
        Id = Id,
        Title = Title,
        Category = Category,
        CoverUrl = CoverUrl,
        AuthorName = AuthorName,
        DurationMinutes = DurationMinutes,
        Rating = Rating,
        IsNew = IsNew
    };
}
