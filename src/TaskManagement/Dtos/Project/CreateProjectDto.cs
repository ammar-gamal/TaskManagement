namespace TaskManagement.Dtos.Project;

public class CreateProjectDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}