using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using LmsApp.Services.Interfaces;
using LmsApp.Services.Implementations;
using LmsApp.Infrastructure;
using LmsApp.Views;
using LmsApp.ViewModels;
using QuestPDF.Infrastructure;

namespace LmsApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
#if !ANDROID
        QuestPDF.Settings.License = LicenseType.Community;
#endif

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterViews(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalRepository, LocalRepository>();
        services.AddSingleton<IApiClient, MockApiClient>();
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INotificationService, NotificationService>();

        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<ILessonService, LessonService>();
        services.AddTransient<IQuizService, QuizService>();
        services.AddTransient<IProgressService, ProgressService>();
        services.AddTransient<IGamificationService, GamificationService>();
        services.AddTransient<ICertificateService, CertificateService>();
        services.AddTransient<IAssignmentService, AssignmentService>();
        services.AddTransient<ITeamService, TeamService>();
        services.AddTransient<ISettingsService, SettingsService>();
    }

    static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<SplashViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<CatalogViewModel>();
        services.AddTransient<CourseDetailViewModel>();
        services.AddTransient<LessonViewModel>();
        services.AddTransient<QuizViewModel>();
        services.AddTransient<QuizResultViewModel>();
        services.AddTransient<MyProgressViewModel>();
        services.AddTransient<CertificatesViewModel>();
        services.AddTransient<AchievementsViewModel>();
        services.AddTransient<LeaderboardViewModel>();
        services.AddTransient<TeamDashboardViewModel>();
        services.AddTransient<AssignCourseViewModel>();
        services.AddTransient<ProfileViewModel>();
    }

    static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<SplashPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<HomePage>();
        services.AddTransient<CatalogPage>();
        services.AddTransient<CourseDetailPage>();
        services.AddTransient<LessonPage>();
        services.AddTransient<QuizPage>();
        services.AddTransient<QuizResultPage>();
        services.AddTransient<MyProgressPage>();
        services.AddTransient<CertificatesPage>();
        services.AddTransient<AchievementsPage>();
        services.AddTransient<LeaderboardPage>();
        services.AddTransient<TeamDashboardPage>();
        services.AddTransient<AssignCoursePage>();
        services.AddTransient<ProfilePage>();
    }
}
