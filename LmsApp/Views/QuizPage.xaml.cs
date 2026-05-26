using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class QuizPage : ContentPage
{
    public QuizPage(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
