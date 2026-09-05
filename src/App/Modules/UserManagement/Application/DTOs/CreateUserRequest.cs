namespace App.Modules.UserManagement.Application.DTOs;

public sealed record CreateUserRequest(
    string Email,
    string Password,
    string? DisplayName,
    IReadOnlyList<Guid> RoleIds);
