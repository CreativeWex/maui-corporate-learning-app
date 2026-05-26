using LmsApp.Helpers;
using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class AchievementsPage : ContentPage
{
    public AchievementsPage(AchievementsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        Nav.AttachBackButton(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AchievementsViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
