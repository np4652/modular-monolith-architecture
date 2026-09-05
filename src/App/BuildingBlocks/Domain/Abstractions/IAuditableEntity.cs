namespace App.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Implemented by entities whose creation/modification must be tracked.
/// Populated centrally by the auditing EF interceptor - never set manually.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; }

    string? CreatedBy { get; }

    DateTime? ModifiedAtUtc { get; }

    string? ModifiedBy { get; }

    void SetCreated(DateTime occurredOnUtc, string? actor);

    void SetModified(DateTime occurredOnUtc, string? actor);
}
