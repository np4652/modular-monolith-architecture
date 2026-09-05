using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using App.BuildingBlocks.Infrastructure.Services;
using App.Modules.UserManagement.Presentation;
using Microsoft.AspNetCore.Authentication;
using IAuthenticationService = App.Modules.UserManagement.Application.Services.IAuthenticationService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Account;

[AllowAnonymous]
public sealed class LoginModel : PageModel
{
    private readonly IAuthenticationService _authenticationService;

    public LoginModel(IAuthenticationService authenticationService) => _authenticationService = authenticationService;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authenticationService.LoginAsync(Input.Email, Input.Password);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
            new(ClaimTypes.Name, result.DisplayName ?? result.Email),
            new(ClaimTypes.Email, result.Email),
        };
        claims.AddRange(result.Permissions.Select(p =>
            new Claim(HttpContextCurrentUserService.PermissionClaimType, p)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return LocalRedirect(returnUrl is { Length: > 0 } ? returnUrl : ModuleRoute.UsersIndex);
    }

    public sealed class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
