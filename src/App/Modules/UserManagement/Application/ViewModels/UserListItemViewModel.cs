namespace App.Modules.UserManagement.Application.ViewModels;

public sealed record UserListItemViewModel(
    Guid Id,
    string Email,
    string? DisplayName,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> RoleNames);
