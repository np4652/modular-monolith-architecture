using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UserManagementDbContext _context;

    public RefreshTokenRepository(UserManagementDbContext context) => _context = context;

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default) =>
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public async Task RevokeAllForUserAsync(Guid userId, DateTime occurredOnUtc, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
            token.Revoke(occurredOnUtc);
    }
}
