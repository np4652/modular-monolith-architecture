namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// Placeholder step for a future MFA challenge. No MFA mechanism is part of this
/// specification, so it currently always continues the chain - kept here so adding
/// MFA later only means replacing this one step, not the pipeline's shape.
/// </summary>
public sealed class CheckMfaIfEnabledStep : IAuthenticationStep
{
    public Task HandleAsync(AuthenticationPipelineContext context, Func<Task> next, CancellationToken cancellationToken) =>
        next();
}
