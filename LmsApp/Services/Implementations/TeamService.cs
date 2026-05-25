using LmsApp.Infrastructure;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class TeamService : ITeamService
{
    private readonly ILocalRepository _repo;
    private readonly ICourseService _courseService;

    public TeamService(ILocalRepository repo, ICourseService courseService)
    {
        _repo = repo;
        _courseService = courseService;
    }

    public async Task<TeamStats> GetTeamStatsAsync(int managerId)
    {
        var members = await BuildTeamMembersAsync(managerId);
        var manager = await _repo.GetUserByIdAsync(managerId);
        return new TeamStats
        {
            TotalMembers = members.Count,
            AvgProgress = members.Any() ? (int)members.Average(m => m.OverallProgress) : 0,
            AssignedCount = members.Sum(m => m.AssignedCourses),
            OverdueCount = members.Count(m => m.NearestDeadline.HasValue && m.NearestDeadline < DateTime.Today),
            AtRiskCount = members.Count(m => m.IsAtRisk),
            DepartmentName = manager?.Department ?? string.Empty
        };
    }

    public async Task<List<TeamMember>> GetTeamMembersAsync(int managerId)
        => await BuildTeamMembersAsync(managerId);

    public async Task<List<TeamMember>> GetAtRiskMembersAsync(int managerId)
    {
        var all = await BuildTeamMembersAsync(managerId);
        return all.Where(m => m.IsAtRisk).ToList();
    }

    async Task<List<TeamMember>> BuildTeamMembersAsync(int managerId)
    {
        var users = await _repo.GetUsersByManagerIdAsync(managerId);
        var result = new List<TeamMember>();

        foreach (var user in users)
        {
            var assignments = await _repo.GetAssignmentsByUserIdAsync(user.Id);
            var modules = new List<int>();
            int totalProgress = 0;
            DateTime? nearestDeadline = null;

            foreach (var a in assignments)
            {
                var progress = await _courseService.GetCourseProgressAsync(user.Id, a.CourseId);
                totalProgress += progress;
                var deadline = DateTime.Parse(a.DeadlineDate);
                if (!nearestDeadline.HasValue || deadline < nearestDeadline)
                    nearestDeadline = deadline;
            }

            int avgProgress = assignments.Any() ? totalProgress / assignments.Count : 0;
            bool atRisk = avgProgress < 30 && nearestDeadline.HasValue &&
                          (nearestDeadline.Value - DateTime.Today).TotalDays <= 3;

            result.Add(new TeamMember
            {
                UserId = user.Id,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Position = user.Position,
                AssignedCourses = assignments.Count,
                CompletedCourses = 0,
                OverallProgress = avgProgress,
                IsAtRisk = atRisk,
                NearestDeadline = nearestDeadline
            });
        }
        return result;
    }
}
