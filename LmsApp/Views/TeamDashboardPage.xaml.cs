using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class TeamDashboardPage : ContentPage
{
    public TeamDashboardPage(TeamDashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TeamDashboardViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
