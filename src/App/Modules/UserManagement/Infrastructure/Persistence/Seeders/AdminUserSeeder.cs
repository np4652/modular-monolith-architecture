using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement.Infrastructure.Persistence.Seeders;

/// <summary>
/// Optionally seeds the very first administrator account from configuration
/// (Seed:AdminEmail / Seed:AdminPassword - supply these via user-secrets or an
/// environment variable, never commit a real credential to appsettings). Skipped
/// entirely when not configured, since there is otherwise no safe default password.
/// </summary>
public static class AdminUserSeeder
{
    public static async Task SeedAsync(
        UserManagementDbContext context,
        IConfiguration configuration,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken = default)
    {
        var adminEmail = configuration["Seed:AdminEmail"];
        var adminPassword = configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        var email = Email.Create(adminEmail);
        if (await context.Users.AnyAsync(u => u.Email == email, cancellationToken))
            return;

        var administratorRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == RoleSeeder.AdministratorRoleName, cancellationToken)
            ?? throw new InvalidOperationException(
                $"'{RoleSeeder.AdministratorRoleName}' role must be seeded before the admin user.");

        var passwordHash = PasswordHash.FromHash(passwordHasher.Hash(adminPassword));
        var admin = User.Register(email, passwordHash, "Administrator");
        admin.AssignRole(administratorRole);

        await context.Users.AddAsync(admin, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
