using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Application;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.UserManagement.Presentation.Components.NavigationMenu;

public sealed record NavigationLink(string Text, string Page);

/// <summary>
/// Renders the module's own navigation links, hidden per-link when the current user
/// lacks the permission - a usability nicety only; the server still enforces access
/// on the destination page regardless of what this menu shows.
/// </summary>
public sealed class NavigationMenuViewComponent : ViewComponent
{
    private readonly ICurrentUserService _currentUserService;

    public NavigationMenuViewComponent(ICurrentUserService currentUserService) =>
        _currentUserService = currentUserService;

    public IViewComponentResult Invoke()
    {
        var links = new List<NavigationLink>();

        if (_currentUserService.HasPermission(UserManagementPermissions.UsersView))
            links.Add(new NavigationLink("Users", "/Users/Index"));

        if (_currentUserService.HasPermission(UserManagementPermissions.RolesView))
            links.Add(new NavigationLink("Roles", "/Roles/Index"));

        return View(links);
    }
}
