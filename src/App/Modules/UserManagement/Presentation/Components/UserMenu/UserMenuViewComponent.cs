using App.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.UserManagement.Presentation.Components.UserMenu;

public sealed record CurrentUserSummary(bool IsAuthenticated, string? DisplayName);

public sealed class UserMenuViewComponent : ViewComponent
{
    private readonly ICurrentUserService _currentUserService;

    public UserMenuViewComponent(ICurrentUserService currentUserService) => _currentUserService = currentUserService;

    public IViewComponentResult Invoke() =>
        View(new CurrentUserSummary(_currentUserService.IsAuthenticated, _currentUserService.UserName));
}
