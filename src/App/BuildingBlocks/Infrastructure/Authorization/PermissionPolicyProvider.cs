using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace App.BuildingBlocks.Infrastructure.Authorization;

/// <summary>
/// Creates an authorization policy on demand for any policy name of the form
/// "Permission:{code}", so a new permission never requires a new policy registration.
/// </summary>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "Permission:";

    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) =>
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.Ordinal))
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);

        var permission = policyName[PolicyPrefix.Length..];
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permission))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
