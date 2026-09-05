using App.Modules.UserManagement.Domain.Enums;
using App.Modules.UserManagement.Domain.Exceptions;

namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// Rejects sign-in for an account an administrator has deactivated.
/// </summary>
public sealed class CheckAccountStatusStep : IAuthenticationStep
{
    public Task HandleAsync(AuthenticationPipelineContext context, Func<Task> next, CancellationToken cancellationToken)
    {
        if (context.User!.Status != UserStatus.Active)
            throw new AccountNotActiveException();

        return next();
    }
}
