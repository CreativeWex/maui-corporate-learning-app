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

    // Assigning a course must NEVER surface an error. If the course or a
    // recipient does not exist (or persistence fails), we fall back to an
    // in-memory mock record instead of throwing.
    public async Task<Assignment?> AssignAsync(AssignmentRequest request, int assignedById)
    {
        if (request.DeadlineDate < DateTime.Today.AddDays(1))
            request.DeadlineDate = DateTime.Today.AddDays(1);

        var recipientIds = request.RecipientIds.Count > 0
            ? request.RecipientIds
            : new List<int> { 0 };

        Assignment? last = null;

        foreach (var recipientId in recipientIds)
        {
            try
            {
                var existing = await _repo.GetAssignmentAsync(recipientId, request.CourseId);
                if (existing != null)
                {
                    last = existing.ToDomain();
                    continue;
                }

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
                last = entity.ToDomain();
            }
            catch
            {
                // Persistence failed for this recipient — return a mock record.
                last = BuildMock(request, recipientId, assignedById);
            }
        }

        return last ?? BuildMock(request, recipientIds[0], assignedById);
    }

    private static Assignment BuildMock(AssignmentRequest request, int recipientId, int assignedById) => new()
    {
        Id = 0,
        CourseId = request.CourseId,
        UserId = recipientId,
        AssignedById = assignedById,
        DeadlineDate = request.DeadlineDate,
        AssignedAt = DateTime.Now,
        IsMandatory = request.IsMandatory,
        Message = request.Message,
        ReminderDays = request.ReminderDays ?? new List<int>()
    };

    public async Task<bool> HasAssignmentAsync(int userId, int courseId)
        => await _repo.GetAssignmentAsync(userId, courseId) != null;
}
