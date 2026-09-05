namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Exposes the identity of the caller to Application/Domain code without those layers
/// taking a dependency on HttpContext.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? UserName { get; }

    bool IsAuthenticated { get; }

    IReadOnlyCollection<string> Permissions { get; }

    bool HasPermission(string permission);
}
