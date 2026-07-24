using Microsoft.EntityFrameworkCore;
using TaskManagement.Entities;

namespace TaskManagement.Persistence.Interfaces;

public interface IAppDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<TaskItem> Tasks { get; }
    public DbSet<Project> Projects { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public int SaveChanges();

}
