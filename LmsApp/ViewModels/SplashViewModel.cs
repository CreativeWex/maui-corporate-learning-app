using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Infrastructure;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class SplashViewModel : BaseViewModel
{
    private readonly ILocalRepository _repo;
    private readonly IAuthService _auth;
    private readonly ISessionService _session;

    [ObservableProperty]
    private string _version = AppInfo.VersionString;

    public SplashViewModel(ILocalRepository repo, IAuthService auth, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _repo = repo;
        _auth = auth;
        _session = session;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await _repo.InitAsync();

        var minShowTask = Task.Delay(1500);

        var user = await _auth.GetSavedUserAsync();
        var token = await _auth.GetSavedTokenAsync();

        await minShowTask;

        if (user != null && !string.IsNullOrEmpty(token))
        {
            _session.SetSession(user, token);
            await NavigateByRole(user.Role);
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
    }

    static Task NavigateByRole(UserRole role) => role switch
    {
        UserRole.Manager or UserRole.Admin => Shell.Current.GoToAsync("//manager"),
        UserRole.Hr                        => Shell.Current.GoToAsync("//hr"),
        _                                  => Shell.Current.GoToAsync("//employee")
    };
}
