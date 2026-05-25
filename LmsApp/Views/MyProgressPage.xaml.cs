using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class MyProgressPage : ContentPage
{
    public MyProgressPage(MyProgressViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MyProgressViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
