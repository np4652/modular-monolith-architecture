using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Captures the domain events raised by tracked aggregates and dispatches them
/// in-process only once the surrounding SaveChanges has actually committed -
/// the implicit "Unit of Work" boundary for a module's DbContext.
/// </summary>
public sealed class DomainEventDispatchInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;
    private List<IDomainEvent> _pendingEvents = [];

    public DomainEventDispatchInterceptor(IDomainEventDispatcher dispatcher) => _dispatcher = dispatcher;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Capture(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Capture(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await FlushAsync(cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    private void Capture(DbContext? context)
    {
        if (context is null) return;

        var aggregatesWithEvents = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        _pendingEvents = aggregatesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        foreach (var aggregate in aggregatesWithEvents)
            aggregate.ClearDomainEvents();
    }

    private async Task FlushAsync(CancellationToken cancellationToken)
    {
        if (_pendingEvents.Count == 0) return;

        var events = _pendingEvents;
        _pendingEvents = [];

        await _dispatcher.DispatchAsync(events, cancellationToken);
    }
}
