using App.Modules.UserManagement.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages;

/// <summary>
/// The application has no generic "home" content of its own - "/" is just a routing
/// shim into whichever module owns the actual landing experience.
/// </summary>
public sealed class IndexModel : PageModel
{
    public IActionResult OnGet() =>
        RedirectToPage(User.Identity?.IsAuthenticated == true ? ModuleRoute.UsersIndex : ModuleRoute.Login);
}
