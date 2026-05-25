using LmsApp.Models.Domain;

namespace LmsApp.Services.Interfaces;

public interface ICertificateService
{
    Task<List<Certificate>> GetCertificatesAsync(int userId);
    Task<Certificate?> GenerateAsync(int userId, int courseId);
    Task<byte[]?> GetPdfBytesAsync(int certId);
}
