using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Entities;
using TaskManagement.Persistence.Configs.Abstractions;

namespace TaskManagement.Persistence.Configs;

public class TaskItemConfig : EntityConfig<TaskItem>
{
    public override void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        base.Configure(builder);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Project)
               .WithMany(e => e.Tasks)
               .HasForeignKey(e => e.ProjectId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(t => t.Description)
               .IsRequired(false)
               .HasMaxLength(2000);

        builder.Property(t => t.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(t => t.Priority)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(e => e.DueDate)
               .IsRequired(false);
        builder.HasIndex(e => e.ProjectId)
               .HasFilter("[DeletedAt] IS NULL");
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Priority);
        builder.HasIndex(e => e.DueDate);
        builder.HasIndex(e => e.CreatedAt);
    }
}
