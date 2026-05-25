using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class LessonPage : ContentPage
{
    public LessonPage(LessonViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
