using System.ComponentModel.DataAnnotations;
using App.BuildingBlocks.Infrastructure.Authorization;
using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Application.ViewModels;
using App.Modules.UserManagement.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Roles;

[RequirePermission(UserManagementPermissions.RolesManage)]
public sealed class EditModel : PageModel
{
    private readonly IRoleService _roleService;

    public EditModel(IRoleService roleService) => _roleService = roleService;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public RoleEditViewModel? Role { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Role = await _roleService.GetForEditAsync(id);
        if (Role is null)
            return NotFound();

        Input = new InputModel
        {
            RoleId = Role.Id,
            Name = Role.Name,
            Description = Role.Description,
            PermissionCodes = Role.Permissions.Where(p => p.Selected).Select(p => p.Code).ToList(),
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Role = await _roleService.GetForEditAsync(Input.RoleId);
            return Page();
        }

        await _roleService.UpdateAsync(new UpdateRoleRequest(
            Input.RoleId, Input.Name, Input.Description, Input.PermissionCodes ?? []));

        return RedirectToPage(ModuleRoute.RolesIndex);
    }

    public sealed class InputModel
    {
        public Guid RoleId { get; set; }

        [Required, MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Description { get; set; }

        public List<string>? PermissionCodes { get; set; }
    }
}
