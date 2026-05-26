using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Helpers;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    // Shared, role-safe back navigation. Bound by the Shell back arrow
    // (BackButtonBehavior) and the "←" toolbar items on pushed pages.
    [RelayCommand]
    protected virtual Task GoBackAsync() => Nav.GoBackAsync();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    protected IDialogService? DialogService { get; }

    protected BaseViewModel(IDialogService? dialogService = null)
    {
        DialogService = dialogService;
    }

    protected async Task RunSafeAsync(Func<Task> action)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            if (DialogService != null)
                await DialogService.ShowAlertAsync("Ошибка", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> RunSafeAsync<T>(Func<Task<T>> action)
    {
        if (IsBusy) return default;
        IsBusy = true;
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            if (DialogService != null)
                await DialogService.ShowAlertAsync("Ошибка", ex.Message);
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
