using App.Modules.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.UserManagement.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Description).HasMaxLength(256);

        builder.HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey(p => p.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Permissions).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(r => r.DomainEvents);
    }
}
