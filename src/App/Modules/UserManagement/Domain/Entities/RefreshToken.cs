using App.BuildingBlocks.Domain.Primitives;

namespace App.Modules.UserManagement.Domain.Entities;

/// <summary>
/// A long-lived, single-use token issued alongside a browser session so it can be
/// revoked independently of the cookie's own expiration.
/// </summary>
public sealed class RefreshToken : AggregateRoot<Guid>
{
    private RefreshToken()
    {
    }

    private RefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc) : base(Guid.NewGuid())
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid UserId { get; private init; }

    public string TokenHash { get; private init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; private init; }

    public DateTime CreatedAtUtc { get; private init; }

    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsActive(DateTime asOfUtc) => RevokedAtUtc is null && ExpiresAtUtc > asOfUtc;

    public static RefreshToken Issue(Guid userId, string tokenHash, DateTime expiresAtUtc) =>
        new(userId, tokenHash, expiresAtUtc);

    public void Revoke(DateTime occurredOnUtc) => RevokedAtUtc ??= occurredOnUtc;
}
