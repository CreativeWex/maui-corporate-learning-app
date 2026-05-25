using System.Text.Json;
using LmsApp.Infrastructure;
using LmsApp.Models.Domain;
using LmsApp.Models.Dto;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class AuthService : IAuthService
{
    private const string TokenKey = "auth_token";
    private const string UserKey = "auth_user";
    private const string AttemptsKey = "login_attempts";
    private const string LockoutKey = "lockout_until";
    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly IApiClient _api;

    public AuthService(IApiClient api) => _api = api;

    public bool IsLockedOut => LockoutUntil.HasValue && LockoutUntil.Value > DateTime.UtcNow;

    public DateTime? LockoutUntil
    {
        get
        {
            var s = Preferences.Get(LockoutKey, string.Empty);
            return string.IsNullOrEmpty(s) ? null : DateTime.Parse(s);
        }
    }

    public int AttemptsLeft => Math.Max(0, MaxAttempts - Preferences.Get(AttemptsKey, 0));

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        if (IsLockedOut) return null;

        var response = await _api.LoginAsync(request);
        if (response == null)
        {
            var attempts = Preferences.Get(AttemptsKey, 0) + 1;
            Preferences.Set(AttemptsKey, attempts);
            if (attempts >= MaxAttempts)
                Preferences.Set(LockoutKey, DateTime.UtcNow.Add(LockoutDuration).ToString("O"));
            return null;
        }

        Preferences.Set(AttemptsKey, 0);
        Preferences.Remove(LockoutKey);
        await SecureStorage.SetAsync(TokenKey, response.Token);
        await SecureStorage.SetAsync(UserKey, JsonSerializer.Serialize(response.User));
        return response;
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(UserKey);
        await _api.LogoutAsync();
    }

    public async Task<string?> GetSavedTokenAsync()
        => await SecureStorage.GetAsync(TokenKey);

    public async Task<User?> GetSavedUserAsync()
    {
        var json = await SecureStorage.GetAsync(UserKey);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<User>(json);
    }
}
