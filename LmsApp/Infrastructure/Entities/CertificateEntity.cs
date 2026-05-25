using SQLite;
using LmsApp.Models.Domain;

namespace LmsApp.Infrastructure.Entities;

[Table("certificates")]
public class CertificateEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CourseId { get; set; }

    [Indexed]
    public int UserId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string IssuedAt { get; set; } = string.Empty;
    public string? PdfPath { get; set; }
    public byte[]? PdfData { get; set; }
    public byte[]? QrCode { get; set; }

    public Certificate ToDomain() => new()
    {
        Id = Id,
        CourseId = CourseId,
        UserId = UserId,
        CourseName = CourseName,
        UserName = UserName,
        IssuedAt = DateTime.Parse(IssuedAt),
        PdfPath = PdfPath,
        PdfData = PdfData,
        QrCode = QrCode
    };
}
