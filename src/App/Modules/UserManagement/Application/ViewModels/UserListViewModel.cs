using App.BuildingBlocks.Application.Common;

namespace App.Modules.UserManagement.Application.ViewModels;

public sealed record UserListViewModel(PagedResult<UserListItemViewModel> Page, string? SearchTerm);
