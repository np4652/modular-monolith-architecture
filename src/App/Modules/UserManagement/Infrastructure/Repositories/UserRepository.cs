using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Domain.ValueObjects;
using App.Modules.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UserManagementDbContext _context;

    public UserRepository(UserManagementDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByEmailWithRolesAsync(Email email, CancellationToken cancellationToken = default) =>
        _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<(IReadOnlyList<User> Users, int TotalCount)> SearchAsync(
        string? searchTerm,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Include(u => u.Roles).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(u =>
                EF.Functions.ILike(EF.Property<string>(u, nameof(User.Email)), pattern) ||
                (u.DisplayName != null && EF.Functions.ILike(u.DisplayName, pattern)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.Email)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entry = _context.Entry(user);
        if (entry.State == EntityState.Detached)
            await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Remove(User user) => _context.Users.Remove(user);
}
