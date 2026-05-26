using LmsApp.Helpers;
using LmsApp.ViewModels;

namespace LmsApp.Views;

public partial class CertificatesPage : ContentPage
{
    public CertificatesPage(CertificatesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        Nav.AttachBackButton(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CertificatesViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
