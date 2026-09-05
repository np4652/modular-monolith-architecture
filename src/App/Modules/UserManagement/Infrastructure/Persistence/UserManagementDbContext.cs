using App.BuildingBlocks.Infrastructure.Persistence;
using App.Modules.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement.Infrastructure.Persistence;

public sealed class UserManagementDbContext : ModuleDbContextBase
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("user_management");

        // Scoped to this module's own namespace so a single-assembly deployment never
        // picks up another module's entity configurations by accident.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UserManagementDbContext).Assembly,
            type => type.Namespace?.StartsWith("App.Modules.UserManagement.", StringComparison.Ordinal) == true);

        base.OnModelCreating(modelBuilder);
    }
}
