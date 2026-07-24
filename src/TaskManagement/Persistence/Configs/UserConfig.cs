using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Entities;
using TaskManagement.Persistence.Configs.Abstractions;

namespace TaskManagement.Persistence.Configs;

public class UserConfig : EntityConfig<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Username)
               .HasMaxLength(50)
               .IsRequired();


        builder.HasIndex(e => e.Username)
               .IsUnique();

    }
}
