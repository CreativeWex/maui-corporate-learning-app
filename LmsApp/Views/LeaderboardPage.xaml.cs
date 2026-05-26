using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class LeaderboardPage : ContentPage
{
    public LeaderboardPage(LeaderboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LeaderboardViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
