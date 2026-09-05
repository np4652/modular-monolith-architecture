using App.BuildingBlocks.Domain.Abstractions;

namespace App.Modules.UserManagement.Domain.Events;

public sealed record UserLoggedInEvent(Guid UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
