using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Turns a physical delete of an <see cref="ISoftDeletable"/> entity into an update
/// that flags it as deleted, so modules never have to remember to soft-delete manually.
/// </summary>
public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public SoftDeleteInterceptor(IDateTimeProvider dateTimeProvider) => _dateTimeProvider = dateTimeProvider;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted) continue;

            entry.State = EntityState.Modified;
            entry.Entity.MarkDeleted(_dateTimeProvider.UtcNow);
        }
    }
}
