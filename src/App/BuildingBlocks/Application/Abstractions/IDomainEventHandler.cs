using App.BuildingBlocks.Domain.Abstractions;

namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Handles a single domain event type in-process. A module registers its own
/// handlers for events raised inside that module, or for events it subscribes
/// to from another module.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
