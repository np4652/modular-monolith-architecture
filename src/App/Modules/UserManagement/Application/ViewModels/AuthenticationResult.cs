namespace App.Modules.UserManagement.Application.ViewModels;

/// <summary>
/// What the PageModel needs to establish a signed-in session. Issuing the actual
/// authentication cookie stays a Presentation concern (HttpContext.SignInAsync);
/// Application only decides who is allowed in and what they can do.
/// </summary>
public sealed record AuthenticationResult(
    Guid UserId,
    string Email,
    string? DisplayName,
    IReadOnlyList<string> Permissions);
