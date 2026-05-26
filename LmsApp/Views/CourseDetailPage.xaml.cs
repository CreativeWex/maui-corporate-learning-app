using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class CourseDetailPage : ContentPage
{
    public CourseDetailPage(CourseDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CourseDetailViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
