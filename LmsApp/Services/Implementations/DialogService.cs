using CommunityToolkit.Maui.Alerts;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class DialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
        => Application.Current?.Windows[0].Page?.DisplayAlertAsync(title, message, cancel)
           ?? Task.CompletedTask;

    public Task<bool> ShowConfirmAsync(string title, string message, string accept = "Да", string cancel = "Нет")
        => Application.Current?.Windows[0].Page?.DisplayAlertAsync(title, message, accept, cancel)
           ?? Task.FromResult(false);

    public async Task ShowToastAsync(string message)
    {
        var toast = Toast.Make(message);
        await toast.Show();
    }
}
