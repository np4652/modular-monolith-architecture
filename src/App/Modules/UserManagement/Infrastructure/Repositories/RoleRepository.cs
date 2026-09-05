using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement.Infrastructure.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly UserManagementDbContext _context;

    public RoleRepository(UserManagementDbContext context) => _context = context;

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _context.Roles.AnyAsync(r => r.Name == name, cancellationToken);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Roles.Include(r => r.Permissions).OrderBy(r => r.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default) =>
        await _context.Roles.AddAsync(role, cancellationToken);

    public void Remove(Role role) => _context.Roles.Remove(role);
}
