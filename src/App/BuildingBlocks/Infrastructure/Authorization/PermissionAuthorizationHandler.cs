using App.BuildingBlocks.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace App.BuildingBlocks.Infrastructure.Authorization;

/// <summary>
/// The server-side, authoritative check behind every permission policy. The UI may
/// additionally hide controls for usability, but this handler is what actually decides.
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(HttpContextCurrentUserService.PermissionClaimType, requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
