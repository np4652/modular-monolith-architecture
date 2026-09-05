namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// Chain of Responsibility for the login workflow:
/// CheckAccountLockout -> ValidateCredentials -> CheckAccountStatus -> CheckMfaIfEnabled -> GenerateAuthenticationResult.
/// The order is fixed here rather than left to DI registration order.
/// </summary>
public sealed class AuthenticationPipeline
{
    private readonly IReadOnlyList<IAuthenticationStep> _orderedSteps;

    public AuthenticationPipeline(
        CheckAccountLockoutStep checkAccountLockout,
        ValidateCredentialsStep validateCredentials,
        CheckAccountStatusStep checkAccountStatus,
        CheckMfaIfEnabledStep checkMfaIfEnabled,
        GenerateAuthenticationResultStep generateAuthenticationResult)
    {
        _orderedSteps =
        [
            checkAccountLockout,
            validateCredentials,
            checkAccountStatus,
            checkMfaIfEnabled,
            generateAuthenticationResult,
        ];
    }

    public async Task<AuthenticationPipelineContext> RunAsync(
        string email,
        string password,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var context = new AuthenticationPipelineContext(email, password, nowUtc);

        Func<Task> terminal = () => Task.CompletedTask;

        var invoke = _orderedSteps
            .Reverse()
            .Aggregate(terminal, (next, step) => () => step.HandleAsync(context, next, cancellationToken));

        await invoke();

        return context;
    }
}
