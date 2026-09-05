using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.UserManagement.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasConversion(e => e.Value, v => Email.Create(v))
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasConversion(h => h.Value, v => PasswordHash.FromHash(v))
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.DisplayName).HasMaxLength(128);

        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity(join => join.ToTable("user_roles"));

        builder.Navigation(u => u.Roles).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(u => u.DomainEvents);

        // Soft-delete query filter is applied centrally in ModuleDbContextBase.
    }
}
