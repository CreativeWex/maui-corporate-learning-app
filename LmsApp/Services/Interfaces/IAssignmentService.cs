using LmsApp.Models.Domain;
using LmsApp.Models.Dto;

namespace LmsApp.Services.Interfaces;

public interface IAssignmentService
{
    Task<List<Assignment>> GetAssignmentsAsync(int userId);
    Task<Assignment?> AssignAsync(AssignmentRequest request, int assignedById);
    Task<bool> HasAssignmentAsync(int userId, int courseId);
}
