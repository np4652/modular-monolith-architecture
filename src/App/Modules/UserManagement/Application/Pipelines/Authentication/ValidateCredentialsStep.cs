using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Domain.Exceptions;

namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// Verifies the supplied password against the stored hash. A wrong password counts
/// as a failed attempt against the account's lockout policy; the caller is responsible
/// for persisting that even when it goes on to reject the login.
/// </summary>
public sealed class ValidateCredentialsStep : IAuthenticationStep
{
    private readonly IPasswordHasher _passwordHasher;

    public ValidateCredentialsStep(IPasswordHasher passwordHasher) => _passwordHasher = passwordHasher;

    public Task HandleAsync(AuthenticationPipelineContext context, Func<Task> next, CancellationToken cancellationToken)
    {
        var user = context.User!;

        if (!_passwordHasher.Verify(context.Password, user.PasswordHash.Value))
        {
            user.RecordFailedLoginAttempt(context.NowUtc);
            throw new InvalidCredentialsException();
        }

        return next();
    }
}
