namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// The single SaveChanges boundary for a use case. Implicit through the module's own
/// DbContext - Application services depend on this, never on the DbContext itself.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
