using LmsApp.Helpers;
using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class AssignCoursePage : ContentPage
{
    public AssignCoursePage(AssignCourseViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        Nav.AttachBackButton(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AssignCourseViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
