using App.BuildingBlocks.Domain.Abstractions;

namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Dispatches domain events raised by aggregates to their in-process handlers after
/// the owning unit of work has committed successfully.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
