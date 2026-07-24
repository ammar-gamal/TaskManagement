using TaskManagement.Entities.Abstractions;

namespace TaskManagement.Entities;

public class Project : Entity, ISoftDeletable
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? DeletedAt { get; set; }
    public User User { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();
}
