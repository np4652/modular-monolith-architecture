namespace App.Modules.UserManagement.Application.ViewModels;

public sealed record UserDetailsViewModel(
    Guid Id,
    string Email,
    string? DisplayName,
    string Status,
    DateTime? LastLoginAtUtc,
    DateTime CreatedAtUtc,
    IReadOnlyList<RoleSummaryViewModel> Roles);
