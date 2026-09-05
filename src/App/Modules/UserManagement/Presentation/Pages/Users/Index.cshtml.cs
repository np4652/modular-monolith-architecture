using App.BuildingBlocks.Application.Common;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Modules.UserManagement.Presentation.Pages.Users;

public sealed class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService) => _userService = userService;

    public UserListViewModel Users { get; private set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public async Task OnGetAsync()
    {
        Users = await _userService.SearchAsync(Search, new PaginationRequest { Page = PageNumber });
    }
}
