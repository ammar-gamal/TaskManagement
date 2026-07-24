using TaskManagement.Entities.Enums;

namespace TaskManagement.Dtos.Task;

public class CreateTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null;
    public Status? Status { get; set; } = null;
    public Priority? Priority { get; set; } = null;
    public DateOnly? DueDate { get; set; } = null;
}
