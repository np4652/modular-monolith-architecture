using App.BuildingBlocks.Domain.Abstractions;

namespace App.Modules.UserManagement.Domain.Events;

public sealed record UserLockedOutEvent(Guid UserId, DateTime LockedUntilUtc) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
