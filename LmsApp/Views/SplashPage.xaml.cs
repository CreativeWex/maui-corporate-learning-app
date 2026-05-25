using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class SplashPage : ContentPage
{
    private readonly SplashViewModel _vm;

    public SplashPage(SplashViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.InitializeCommand.Execute(null);
    }
}
