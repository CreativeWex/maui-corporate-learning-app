using SQLite;
using LmsApp.Models.Domain;
using LmsApp.Models.Enums;

namespace LmsApp.Infrastructure.Entities;

[Table("users")]
public class UserEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Role { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int? ManagerId { get; set; }
    public int TotalXp { get; set; }
    public int Level { get; set; }

    public static UserEntity FromDomain(User u, string passwordHash) => new()
    {
        Id = u.Id,
        Email = u.Email,
        PasswordHash = passwordHash,
        Name = u.Name,
        Role = (int)u.Role,
        Department = u.Department,
        Position = u.Position,
        AvatarUrl = u.AvatarUrl,
        ManagerId = u.ManagerId
    };

    public User ToDomain() => new()
    {
        Id = Id,
        Email = Email,
        Name = Name,
        Role = (UserRole)Role,
        Department = Department,
        Position = Position,
        AvatarUrl = AvatarUrl,
        ManagerId = ManagerId
    };
}
