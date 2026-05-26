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

    public LoginViewModel(IAuthService auth, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _auth = auth;
        _session = session;
    }

    bool CanLogin => !string.IsNullOrWhiteSpace(Email) &&
                     Password.Length >= 6 &&
                     !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    async Task LoginAsync()
    {
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
                HasError = true;
                ErrorMessage = "Неверный email или пароль.";
                return;
            }

            _session.SetSession(response.User, response.Token);
            NavigateByRole(response.User.Role);
        });
    }

    static void NavigateByRole(UserRole role)
    {
        if (Shell.Current is AppShell appShell)
            appShell.NavigateToRole(role);
    }
}
