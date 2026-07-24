using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Entities;
using TaskManagement.Persistence.Configs.Abstractions;

namespace TaskManagement.Persistence.Configs;

public class ProjectConfig : EntityConfig<Project>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.Name)
               .IsRequired(true)
               .HasMaxLength(500);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasFilter("[DeletedAt] IS NULL");

        builder.Property(e => e.Description)
               .IsRequired(false)
               .HasMaxLength(2000);

        builder.HasOne(e => e.User)
               .WithMany(e => e.Projects)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
