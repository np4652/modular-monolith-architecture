namespace App.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Marker for something that happened in the domain. Dispatched in-process after
/// a successful unit of work, and optionally relayed as an integration event.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTime OccurredOnUtc { get; }
}
