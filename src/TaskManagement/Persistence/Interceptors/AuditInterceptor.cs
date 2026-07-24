using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManagement.Entities.Abstractions;

namespace TaskManagement.Persistence.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }



    private static void UpdateAuditFields(DbContext? context)
    {
        var utcNow = DateTime.UtcNow;
        UpdateTimestamps(context, utcNow);//Must be called before softdeletion because softdeletion convert the state to modified
        HandleSoftDeletion(context, utcNow);
    }
    private static void HandleSoftDeletion(DbContext? context, DateTime utcNow)
    {
        if (context is null)
            return;

        var entries = context.ChangeTracker
                             .Entries<ISoftDeletable>()
                             .Where(e => e.State == EntityState.Deleted);
        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.DeletedAt = utcNow;
        }

    }
    private static void UpdateTimestamps(DbContext? context, DateTime utcNow)
    {
        if (context is null)
            return;

        var entries = context.ChangeTracker
                             .Entries<Entity>()
                             .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }
}
