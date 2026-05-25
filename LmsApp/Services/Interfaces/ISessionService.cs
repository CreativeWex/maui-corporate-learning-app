using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Services.Interfaces;

public interface ISessionService
{
    User? CurrentUser { get; }
    UserRole Role { get; }
    bool IsLoggedIn { get; }
    void SetSession(User user, string token);
    void Clear();
}
