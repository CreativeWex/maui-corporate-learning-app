using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Helpers;
using LmsApp.Models.Dto;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly ISessionService _session;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private int _attemptsLeft = 5;

    [ObservableProperty]
    private bool _isLockedOut;

    [ObservableProperty]
    private string _lockoutMessage = string.Empty;

    public LoginViewModel(IAuthService auth, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _auth = auth;
        _session = session;
    }

    bool CanLogin => !string.IsNullOrWhiteSpace(Email) &&
                     Password.Length >= 6 &&
                     !IsBusy &&
                     !IsLockedOut;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    async Task LoginAsync()
    {
        if (_auth.IsLockedOut)
        {
            var until = _auth.LockoutUntil!.Value.ToLocalTime();
            LockoutMessage = $"Аккаунт заблокирован до {until:HH:mm}";
            IsLockedOut = true;
            return;
        }

        await RunSafeAsync(async () =>
        {
            HasError = false;
            ErrorMessage = string.Empty;

            var response = await _auth.LoginAsync(new LoginRequest
            {
                Email = Email.Trim().ToLowerInvariant(),
                Password = Password
            });

            if (response == null)
            {
                AttemptsLeft = _auth.AttemptsLeft;
                if (_auth.IsLockedOut)
                {
                    IsLockedOut = true;
                    var until = _auth.LockoutUntil!.Value.ToLocalTime();
                    LockoutMessage = $"Слишком много попыток. Заблокировано до {until:HH:mm}";
                }
                else
                {
                    HasError = true;
                    ErrorMessage = $"Неверный email или пароль. Осталось попыток: {AttemptsLeft}";
                }
                return;
            }

            _session.SetSession(response.User, response.Token);
            await NavigateByRole(response.User.Role);
        });
    }

    [RelayCommand]
    void SsoLogin()
    {
        // Decorative stub
    }

    [RelayCommand]
    void ForgotPassword()
    {
        // Decorative stub
    }

    static Task NavigateByRole(UserRole role) => role switch
    {
        UserRole.Manager or UserRole.Admin => Shell.Current.GoToAsync("//manager"),
        UserRole.Hr                        => Shell.Current.GoToAsync("//hr"),
        _                                  => Shell.Current.GoToAsync("//employee")
    };
}
