namespace App.BuildingBlocks.Infrastructure.Authorization;

public sealed record PermissionDescriptor(string Code, string Category, string DisplayName);

/// <summary>
/// A module contributes its own permission catalog by registering one of these,
/// so BuildingBlocks never has to know a module's permission constants up front.
/// </summary>
public interface IPermissionCatalogContributor
{
    IReadOnlyCollection<PermissionDescriptor> GetPermissions();
}

/// <summary>
/// The full, aggregated permission catalog across every module. Consumed by the
/// Roles UI to render the assignable permission list.
/// </summary>
public interface IPermissionRegistry
{
    IReadOnlyCollection<PermissionDescriptor> All { get; }
}

public sealed class PermissionRegistry : IPermissionRegistry
{
    public PermissionRegistry(IEnumerable<IPermissionCatalogContributor> contributors) =>
        All = contributors.SelectMany(c => c.GetPermissions()).ToList().AsReadOnly();

    public IReadOnlyCollection<PermissionDescriptor> All { get; }
}
