namespace App.Modules.UserManagement.Application.DTOs;

public sealed record UpdateUserRequest(
    Guid UserId,
    string? DisplayName,
    bool IsActive,
    IReadOnlyList<Guid> RoleIds);
