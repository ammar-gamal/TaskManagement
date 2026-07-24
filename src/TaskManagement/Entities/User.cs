using TaskManagement.Entities.Abstractions;

namespace TaskManagement.Entities;

public class User : Entity
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
}
