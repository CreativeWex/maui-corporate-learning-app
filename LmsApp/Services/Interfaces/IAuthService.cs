using LmsApp.Models.Domain;
using LmsApp.Models.Dto;

namespace LmsApp.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<string?> GetSavedTokenAsync();
    Task<User?> GetSavedUserAsync();
}
