using App.Modules.UserManagement.Domain.Entities;

namespace App.Modules.UserManagement.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task RevokeAllForUserAsync(Guid userId, DateTime occurredOnUtc, CancellationToken cancellationToken = default);
}
