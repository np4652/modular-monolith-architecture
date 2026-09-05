using App.BuildingBlocks.Domain.Abstractions;

namespace App.Modules.UserManagement.Domain.Events;

public sealed record RoleAssignedToUserEvent(Guid UserId, Guid RoleId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
