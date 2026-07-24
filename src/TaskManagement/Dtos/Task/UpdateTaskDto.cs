using TaskManagement.Entities.Enums;

namespace TaskManagement.Dtos.Task;

public class UpdateTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public DateOnly? DueDate { get; set; }
}
