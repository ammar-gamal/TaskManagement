namespace TaskManagement.Entities.Abstractions;

public interface ISoftDeletable
{
    public DateTime? DeletedAt { get; set; }
}
