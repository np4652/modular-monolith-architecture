using Microsoft.AspNetCore.Authorization;

namespace App.BuildingBlocks.Infrastructure.Authorization;

/// <summary>
/// Applied to a PageModel (or handler) to require a specific permission, e.g.
/// [RequirePermission(UserManagementPermissions.UsersCreate)].
/// </summary>
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission) : base(PermissionPolicyProvider.PolicyPrefix + permission)
    {
    }
}
