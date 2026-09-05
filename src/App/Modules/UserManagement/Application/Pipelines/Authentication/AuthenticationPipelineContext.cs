using App.Modules.UserManagement.Application.ViewModels;
using App.Modules.UserManagement.Domain.Entities;

namespace App.Modules.UserManagement.Application.Pipelines.Authentication;

/// <summary>
/// Mutable state threaded through the authentication chain. Each step reads what it
/// needs and contributes what it produces; only <see cref="GenerateAuthenticationResultStep"/>
/// is expected to populate <see cref="Result"/>.
/// </summary>
public sealed class AuthenticationPipelineContext
{
    public AuthenticationPipelineContext(string email, string password, DateTime nowUtc)
    {
        Email = email;
        Password = password;
        NowUtc = nowUtc;
    }

    public string Email { get; }

    public string Password { get; }

    public DateTime NowUtc { get; }

    public User? User { get; set; }

    public AuthenticationResult? Result { get; set; }
}
