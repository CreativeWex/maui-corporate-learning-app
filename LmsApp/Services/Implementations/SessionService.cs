using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class SessionService : ISessionService
{
    public User? CurrentUser { get; private set; }
    public UserRole Role => CurrentUser?.Role ?? UserRole.Employee;
    public bool IsLoggedIn => CurrentUser != null;

    public void SetSession(User user, string token)
    {
        CurrentUser = user;
    }

    public void Clear()
    {
        CurrentUser = null;
    }
}
