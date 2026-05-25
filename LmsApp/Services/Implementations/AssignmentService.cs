using System.Text.Json;
using LmsApp.Infrastructure;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Domain;
using LmsApp.Models.Dto;
using LmsApp.Services.Interfaces;

namespace LmsApp.Services.Implementations;

public class AssignmentService : IAssignmentService
{
    private readonly ILocalRepository _repo;

    public AssignmentService(ILocalRepository repo) => _repo = repo;

    public async Task<List<Assignment>> GetAssignmentsAsync(int userId)
    {
        var list = await _repo.GetAssignmentsByUserIdAsync(userId);
        return list.Select(a => a.ToDomain()).ToList();
    }

    public async Task<Assignment?> AssignAsync(AssignmentRequest request, int assignedById)
    {
        if (request.DeadlineDate < DateTime.Today.AddDays(1))
            throw new Exception("Дедлайн должен быть не раньше завтра");

        var results = new List<Assignment>();
        foreach (var recipientId in request.RecipientIds)
        {
            var existing = await _repo.GetAssignmentAsync(recipientId, request.CourseId);
            if (existing != null) continue; // skip duplicates

            var entity = new AssignmentEntity
            {
                CourseId = request.CourseId,
                UserId = recipientId,
                AssignedById = assignedById,
                DeadlineDate = request.DeadlineDate.ToString("O"),
                AssignedAt = DateTime.Now.ToString("O"),
                IsMandatory = request.IsMandatory,
                Message = request.Message,
                ReminderDaysJson = JsonSerializer.Serialize(request.ReminderDays)
            };
            await _repo.SaveAssignmentAsync(entity);
        }
        return null;
    }

    public async Task<bool> HasAssignmentAsync(int userId, int courseId)
        => await _repo.GetAssignmentAsync(userId, courseId) != null;
}
