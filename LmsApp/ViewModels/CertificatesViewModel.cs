using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LmsApp.Models.Domain;
using LmsApp.Services.Interfaces;

namespace LmsApp.ViewModels;

public partial class CertificatesViewModel : BaseViewModel
{
    private readonly ICertificateService _certService;
    private readonly ISessionService _session;

    [ObservableProperty] private ObservableCollection<Certificate> _certificates = new();
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private bool _isRefreshing;

    public CertificatesViewModel(ICertificateService certService, ISessionService session, IDialogService dialog)
        : base(dialog)
    {
        _certService = certService;
        _session = session;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var userId = _session.CurrentUser?.Id ?? 0;
            var list = await _certService.GetCertificatesAsync(userId);
            Certificates = new ObservableCollection<Certificate>(list);
            IsEmpty = !list.Any();
        });
    }

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    async Task DownloadCertificateAsync(Certificate cert)
    {
        await RunSafeAsync(async () =>
        {
            var pdfBytes = await _certService.GetPdfBytesAsync(cert.Id);
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                if (DialogService != null)
                    await DialogService.ShowAlertAsync("Ошибка", "PDF недоступен");
                return;
            }

            var fileName = $"Certificate_{cert.CourseName.Replace(" ", "_")}_{cert.IssuedAt:yyyy-MM-dd}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Сертификат: {cert.CourseName}",
                File = new ShareFile(filePath)
            });
        });
    }
}
