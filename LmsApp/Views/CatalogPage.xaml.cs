using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage(CatalogViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CatalogViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
