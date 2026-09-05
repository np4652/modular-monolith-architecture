using App.Modules.UserManagement.Application.ViewModels;

namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// The chain's terminal step: records the successful login and produces what the
/// PageModel needs to sign the user into a cookie session.
/// </summary>
public sealed class GenerateAuthenticationResultStep : IAuthenticationStep
{
    public Task HandleAsync(AuthenticationPipelineContext context, Func<Task> next, CancellationToken cancellationToken)
    {
        var user = context.User!;
        user.RecordSuccessfulLogin(context.NowUtc);

        var permissions = user.Roles
            .SelectMany(r => r.Permissions.Select(p => p.PermissionCode))
            .Distinct()
            .ToList();

        context.Result = new AuthenticationResult(user.Id, user.Email.Value, user.DisplayName, permissions);

        return next();
    }
}
