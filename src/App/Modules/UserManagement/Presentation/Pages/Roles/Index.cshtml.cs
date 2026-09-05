using System.ComponentModel.DataAnnotations;
using App.BuildingBlocks.Infrastructure.Authorization;
using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Application.ViewModels;
using App.Modules.UserManagement.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Roles;

// Only [Authorize]/[RequirePermission] on the PageModel CLASS is enforced by the
// framework - Razor Pages does not apply it per handler method - so the mutating
// handlers below, which need a stricter permission than the page's own
// "roles.view" folder policy, check authorization explicitly instead.
public sealed class IndexModel : PageModel
{
    private readonly IRoleService _roleService;
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IRoleService roleService, IAuthorizationService authorizationService)
    {
        _roleService = roleService;
        _authorizationService = authorizationService;
    }

    public IReadOnlyList<RoleListItemViewModel> Roles { get; private set; } = [];

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Roles = await _roleService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!await CanManageRolesAsync())
            return Forbid();

        if (!ModelState.IsValid)
        {
            Roles = await _roleService.GetAllAsync();
            return Page();
        }

        await _roleService.CreateAsync(new CreateRoleRequest(Input.Name, Input.Description, []));
        return RedirectToPage(ModuleRoute.RolesIndex);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        if (!await CanManageRolesAsync())
            return Forbid();

        await _roleService.DeleteAsync(id);
        return RedirectToPage(ModuleRoute.RolesIndex);
    }

    private async Task<bool> CanManageRolesAsync()
    {
        var result = await _authorizationService.AuthorizeAsync(
            User, PermissionPolicyProvider.PolicyPrefix + UserManagementPermissions.RolesManage);

        return result.Succeeded;
    }

    public sealed class InputModel
    {
        [Required, MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Description { get; set; }
    }
}
