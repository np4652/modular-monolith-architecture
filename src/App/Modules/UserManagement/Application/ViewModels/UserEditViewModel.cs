namespace App.Modules.UserManagement.Application.ViewModels;

public sealed record UserEditViewModel(
    Guid Id,
    string Email,
    string? DisplayName,
    bool IsActive,
    IReadOnlyList<RoleOptionViewModel> Roles);
