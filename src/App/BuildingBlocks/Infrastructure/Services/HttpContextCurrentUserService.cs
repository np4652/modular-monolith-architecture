using System.Security.Claims;
using App.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace App.BuildingBlocks.Infrastructure.Services;

public sealed class HttpContextCurrentUserService : ICurrentUserService
{
    public const string PermissionClaimType = "permission";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? UserName => User?.Identity?.Name;

    public IReadOnlyCollection<string> Permissions =>
        User?.FindAll(PermissionClaimType).Select(c => c.Value).ToArray() ?? [];

    public bool HasPermission(string permission) => User?.HasClaim(PermissionClaimType, permission) ?? false;
}
