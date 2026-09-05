using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement.Infrastructure.Persistence.Seeders;

/// <summary>
/// Ensures the built-in, non-deletable "Administrator" role exists so the first
/// admin account (see <see cref="AdminUserSeeder"/>) has somewhere to attach.
/// </summary>
public static class RoleSeeder
{
    public const string AdministratorRoleName = "Administrator";

    public static async Task SeedAsync(UserManagementDbContext context, CancellationToken cancellationToken = default)
    {
        var exists = await context.Roles.AnyAsync(r => r.Name == AdministratorRoleName, cancellationToken);
        if (exists) return;

        var administrator = Role.CreateSystemRole(
            AdministratorRoleName,
            "Full access to every user management capability.",
            UserManagementPermissions.All.Select(p => p.Code));

        await context.Roles.AddAsync(administrator, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
