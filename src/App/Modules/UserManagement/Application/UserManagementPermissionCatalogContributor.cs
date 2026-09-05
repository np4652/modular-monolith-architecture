using App.BuildingBlocks.Infrastructure.Authorization;

namespace App.Modules.UserManagement.Application;

public sealed class UserManagementPermissionCatalogContributor : IPermissionCatalogContributor
{
    public IReadOnlyCollection<PermissionDescriptor> GetPermissions() => UserManagementPermissions.All;
}
