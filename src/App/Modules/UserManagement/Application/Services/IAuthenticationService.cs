using App.Modules.UserManagement.Application.ViewModels;

namespace App.Modules.UserManagement.Application.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
