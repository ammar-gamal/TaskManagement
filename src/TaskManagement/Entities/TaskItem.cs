using TaskManagement.Entities.Abstractions;
using TaskManagement.Entities.Enums;

namespace TaskManagement.Entities;

public class TaskItem : Entity, ISoftDeletable
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Enums.Status Status { get; set; } = Status.Todo;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateOnly? DueDate { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Project Project { get; set; } = null!;
}
