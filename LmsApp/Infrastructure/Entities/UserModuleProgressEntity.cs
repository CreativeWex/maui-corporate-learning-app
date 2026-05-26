using SQLite;

namespace LmsApp.Infrastructure.Entities;

[Table("UserModuleProgress")]
public class UserModuleProgressEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int UserId { get; set; }
    [Indexed] public int ModuleId { get; set; }
    public int Status { get; set; }
}
