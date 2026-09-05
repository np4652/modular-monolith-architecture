using System.ComponentModel.DataAnnotations;
using App.BuildingBlocks.Infrastructure.Authorization;
using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Users;

[RequirePermission(UserManagementPermissions.UsersCreate)]
public sealed class CreateModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public CreateModel(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<(Guid Id, string Name)> AvailableRoles { get; private set; } = [];

    public async Task OnGetAsync()
    {
        await LoadAvailableRolesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAvailableRolesAsync();
            return Page();
        }

        await _userService.CreateAsync(new CreateUserRequest(
            Input.Email,
            Input.Password,
            Input.DisplayName,
            Input.RoleIds ?? []));

        return RedirectToPage(ModuleRoute.UsersIndex);
    }

    private async Task LoadAvailableRolesAsync()
    {
        var roles = await _roleService.GetAllAsync();
        AvailableRoles = roles.Select(r => (r.Id, r.Name)).ToList();
    }

    public sealed class InputModel
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? DisplayName { get; set; }

        public List<Guid>? RoleIds { get; set; }
    }
}
