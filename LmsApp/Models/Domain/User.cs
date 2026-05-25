using LmsApp.Models.Enums;

namespace LmsApp.Models.Domain;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int? ManagerId { get; set; }
}
