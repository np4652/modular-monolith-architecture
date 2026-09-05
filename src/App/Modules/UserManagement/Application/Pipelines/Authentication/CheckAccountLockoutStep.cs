using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Domain.ValueObjects;

namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// Loads the account by email and rejects the attempt outright if it is currently
/// locked out, before a password is even compared.
/// </summary>
public sealed class CheckAccountLockoutStep : IAuthenticationStep
{
    private readonly IUserRepository _userRepository;

    public CheckAccountLockoutStep(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task HandleAsync(AuthenticationPipelineContext context, Func<Task> next, CancellationToken cancellationToken)
    {
        Email email;
        try
        {
            email = Email.Create(context.Email);
        }
        catch (InvalidEmailException)
        {
            throw new InvalidCredentialsException();
        }

        var user = await _userRepository.GetByEmailWithRolesAsync(email, cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        if (user.IsLockedOut(context.NowUtc))
            throw new AccountLockedException(user.LockedUntilUtc!.Value);

        context.User = user;

        await next();
    }
}
