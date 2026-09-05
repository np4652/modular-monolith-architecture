using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Account;

[AllowAnonymous]
public sealed class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
    }
}
