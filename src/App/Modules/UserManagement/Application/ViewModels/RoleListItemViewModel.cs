namespace App.Modules.UserManagement.Application.ViewModels;

public sealed record RoleListItemViewModel(
    Guid Id,
    string Name,
    string? Description,
    int PermissionCount,
    bool IsSystemRole);
