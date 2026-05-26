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

    private readonly IApiClient _api;

    public AuthService(IApiClient api) => _api = api;

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _api.LoginAsync(request);
        if (response == null) return null;

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
