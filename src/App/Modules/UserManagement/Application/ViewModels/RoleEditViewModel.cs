namespace App.Modules.UserManagement.Application.ViewModels;

public sealed record RoleEditViewModel(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    IReadOnlyList<PermissionOptionViewModel> Permissions);
