using LmsApp.Models.Domain;

namespace LmsApp.Services.Interfaces;

public interface ITeamService
{
    Task<TeamStats> GetTeamStatsAsync(int managerId);
    Task<List<TeamMember>> GetTeamMembersAsync(int managerId);
    Task<List<TeamMember>> GetAtRiskMembersAsync(int managerId);
}
