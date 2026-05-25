using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class QuizResultPage : ContentPage
{
    public QuizResultPage(QuizResultViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
