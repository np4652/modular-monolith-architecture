namespace App.Modules.UserManagement.Application.DTOs;

public sealed record CreateRoleRequest(string Name, string? Description, IReadOnlyList<string> PermissionCodes);
