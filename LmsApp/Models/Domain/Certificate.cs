namespace LmsApp.Models.Domain;

public class Certificate
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int UserId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? PdfPath { get; set; }
    public byte[]? PdfData { get; set; }
    public byte[]? QrCode { get; set; }
    public string UserName { get; set; } = string.Empty;
}
