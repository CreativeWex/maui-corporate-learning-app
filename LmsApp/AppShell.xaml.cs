using LmsApp.Models.Enums;
using LmsApp.Views;

namespace LmsApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Initialize CurrentItem for hidden TabBars so Android doesn't crash
        // when creating Fragments for them during onStart
        if (ManagerTabBar.Items.Count > 0)
            ManagerTabBar.CurrentItem = ManagerTabBar.Items[0];
        if (HrTabBar.Items.Count > 0)
            HrTabBar.CurrentItem = HrTabBar.Items[0];

        Routing.RegisterRoute("course-detail", typeof(CourseDetailPage));
        Routing.RegisterRoute("lesson", typeof(LessonPage));
        Routing.RegisterRoute("quiz", typeof(QuizPage));
        Routing.RegisterRoute("quiz-result", typeof(QuizResultPage));
        Routing.RegisterRoute("certificates", typeof(CertificatesPage));
        Routing.RegisterRoute("achievements", typeof(AchievementsPage));
        Routing.RegisterRoute("leaderboard", typeof(LeaderboardPage));
        Routing.RegisterRoute("team-dashboard", typeof(TeamDashboardPage));
        Routing.RegisterRoute("assign-course", typeof(AssignCoursePage));
    }

    public void NavigateToRole(UserRole role)
    {
        CurrentItem = role switch
        {
            UserRole.Manager or UserRole.Admin => ManagerTabBar,
            UserRole.Hr                        => HrTabBar,
            _                                  => EmployeeTabBar
        };
        _ = Shell.Current.Navigation.PopToRootAsync(false);
    }

    // Handle Android hardware back button for registered-route pages
    protected override bool OnBackButtonPressed()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            _ = Navigation.PopAsync(false);
            return true;
        }
        return base.OnBackButtonPressed();
    }
}
