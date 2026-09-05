using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.ValueObjects;

namespace App.Modules.UserManagement.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailWithRolesAsync(Email email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<User> Users, int TotalCount)> SearchAsync(
        string? searchTerm,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    void Remove(User user);
}
