namespace LmsApp.Services.Interfaces;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmAsync(string title, string message, string accept = "Да", string cancel = "Нет");
    Task ShowToastAsync(string message);
}
