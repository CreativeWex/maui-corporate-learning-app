using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class CourseDetailPage : ContentPage
{
    public CourseDetailPage(CourseDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
