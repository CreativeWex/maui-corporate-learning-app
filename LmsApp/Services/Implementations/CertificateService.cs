using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QColors = QuestPDF.Helpers.Colors;
using QRCoder;
using LmsApp.Infrastructure;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class CertificateService : ICertificateService
{
    private readonly ILocalRepository _repo;
    private readonly ISessionService _session;

    public CertificateService(ILocalRepository repo, ISessionService session)
    {
        _repo = repo;
        _session = session;
    }

    public async Task<List<Certificate>> GetCertificatesAsync(int userId)
    {
        var list = await _repo.GetCertificatesByUserIdAsync(userId);
        return list.Select(c => c.ToDomain()).ToList();
    }

    public async Task<Certificate?> GenerateAsync(int userId, int courseId)
    {
        var existing = await _repo.GetCertificateAsync(userId, courseId);
        if (existing != null) return existing.ToDomain();

        var course = await _repo.GetCourseByIdAsync(courseId);
        var user = await _repo.GetUserByIdAsync(userId);
        if (course == null || user == null) return null;

        var cert = new Certificate
        {
            CourseId = courseId,
            UserId = userId,
            CourseName = course.Title,
            UserName = user.Name,
            IssuedAt = DateTime.Now
        };

        cert.QrCode = GenerateQrCode($"LMS-CERT-{userId}-{courseId}-{cert.IssuedAt:yyyyMMdd}");
        cert.PdfData = GeneratePdf(cert);

        var entity = new CertificateEntity
        {
            CourseId = cert.CourseId,
            UserId = cert.UserId,
            CourseName = cert.CourseName,
            UserName = cert.UserName,
            IssuedAt = cert.IssuedAt.ToString("O"),
            PdfData = cert.PdfData,
            QrCode = cert.QrCode
        };
        await _repo.SaveCertificateAsync(entity);
        cert.Id = entity.Id;
        return cert;
    }

    public async Task<byte[]?> GetPdfBytesAsync(int certId)
    {
        var list = await _repo.GetCertificatesByUserIdAsync(_session.CurrentUser?.Id ?? 0);
        return list.FirstOrDefault(c => c.Id == certId)?.PdfData;
    }

    byte[] GeneratePdf(Certificate cert)
    {
#if ANDROID
        return GenerateSimplePdf(cert);
#else
        try
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.Background().Background(QColors.White);

                    page.Content().Column(col =>
                    {
                        col.Item().AlignCenter().Text("СЕРТИФИКАТ").FontSize(36).Bold().FontColor(QColors.Indigo.Darken3);
                        col.Item().AlignCenter().Text("об успешном прохождении курса").FontSize(16).FontColor(QColors.Grey.Darken1);
                        col.Item().PaddingTop(30).AlignCenter().Text(cert.CourseName).FontSize(26).Bold().FontColor(QColors.Grey.Darken3);
                        col.Item().PaddingTop(20).AlignCenter().Text("Выдан").FontSize(14).FontColor(QColors.Grey.Medium);
                        col.Item().AlignCenter().Text(cert.UserName).FontSize(24).Bold().FontColor(QColors.Grey.Darken3);
                        col.Item().PaddingTop(20).AlignCenter().Text($"Дата выдачи: {cert.IssuedAt:dd.MM.yyyy}").FontSize(14).FontColor(QColors.Grey.Medium);

                        if (cert.QrCode != null)
                        {
                            col.Item().PaddingTop(20).AlignCenter().Width(80).Image(cert.QrCode);
                        }

                        col.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().BorderBottom(1).BorderColor(QColors.Grey.Lighten1).Width(150).AlignCenter().Text(string.Empty);
                                c.Item().AlignCenter().Text("HR-директор").FontSize(12).FontColor(QColors.Grey.Medium);
                            });
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().BorderBottom(1).BorderColor(QColors.Grey.Lighten1).Width(150).AlignCenter().Text(string.Empty);
                                c.Item().AlignCenter().Text("Руководитель обучения").FontSize(12).FontColor(QColors.Grey.Medium);
                            });
                        });
                    });
                });
            }).GeneratePdf();
        }
        catch
        {
            return GenerateSimplePdf(cert);
        }
#endif
    }

    static byte[] GenerateSimplePdf(Certificate cert)
    {
        // Fallback: text-only PDF stub
        var text = $"СЕРТИФИКАТ\n{cert.CourseName}\nВыдан: {cert.UserName}\nДата: {cert.IssuedAt:dd.MM.yyyy}";
        return System.Text.Encoding.UTF8.GetBytes(text);
    }

    static byte[] GenerateQrCode(string text)
    {
        try
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
            using var qr = new PngByteQRCode(data);
            return qr.GetGraphic(4);
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
}
