using LmsApp.Views;

namespace LmsApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

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
}
