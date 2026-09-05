using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Stamps created/modified audit fields on every <see cref="IAuditableEntity"/> just
/// before it is saved, so no module has to do this itself.
/// </summary>
public sealed class AuditingInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public AuditingInterceptor(IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
    {
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

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

        var now = _dateTimeProvider.UtcNow;
        var actor = _currentUserService.UserName;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.SetCreated(now, actor);
            else if (entry.State == EntityState.Modified)
                entry.Entity.SetModified(now, actor);
        }
    }
}
