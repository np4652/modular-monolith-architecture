namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// One responsibility in the login workflow. A step either throws a domain exception
/// to short-circuit the chain (e.g. account locked) or calls <paramref name="next"/>
/// to continue.
/// </summary>
public interface IAuthenticationStep
{
    Task HandleAsync(
        AuthenticationPipelineContext context,
        Func<Task> next,
        CancellationToken cancellationToken);
}
