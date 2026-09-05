using System.ComponentModel.DataAnnotations;
using App.BuildingBlocks.Infrastructure.Authorization;
using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Application.ViewModels;
using App.Modules.UserManagement.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Users;

[RequirePermission(UserManagementPermissions.UsersEdit)]
public sealed class EditModel : PageModel
{
    private readonly IUserService _userService;

    public EditModel(IUserService userService) => _userService = userService;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public UserEditViewModel? UserDetails { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        UserDetails = await _userService.GetForEditAsync(id);
        if (UserDetails is null)
            return NotFound();

        Input = new InputModel
        {
            UserId = UserDetails.Id,
            DisplayName = UserDetails.DisplayName,
            IsActive = UserDetails.IsActive,
            RoleIds = UserDetails.Roles.Where(r => r.Selected).Select(r => r.Id).ToList(),
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            UserDetails = await _userService.GetForEditAsync(Input.UserId);
            return Page();
        }

        await _userService.UpdateAsync(new UpdateUserRequest(
            Input.UserId,
            Input.DisplayName,
            Input.IsActive,
            Input.RoleIds ?? []));

        return RedirectToPage(ModuleRoute.UsersIndex);
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(Guid id, bool activate)
    {
        if (activate) await _userService.ActivateAsync(id);
        else await _userService.DeactivateAsync(id);

        return RedirectToPage(new { id });
    }

    public sealed class InputModel
    {
        public Guid UserId { get; set; }

        [MaxLength(128)]
        public string? DisplayName { get; set; }

        public bool IsActive { get; set; }

        public List<Guid>? RoleIds { get; set; }
    }
}
