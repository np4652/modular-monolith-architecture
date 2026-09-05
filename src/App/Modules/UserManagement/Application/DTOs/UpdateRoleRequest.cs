namespace App.Modules.UserManagement.Application.DTOs;

public sealed record UpdateRoleRequest(
    Guid RoleId,
    string Name,
    string? Description,
    IReadOnlyList<string> PermissionCodes);
