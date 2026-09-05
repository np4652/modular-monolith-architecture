using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Exceptions;
using App.Modules.UserManagement.Application.Pipelines.Authentication;
using App.Modules.UserManagement.Application.ViewModels;

namespace App.Modules.UserManagement.Application.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationPipeline _pipeline;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthenticationService(AuthenticationPipeline pipeline, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _pipeline = pipeline;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var context = await _pipeline.RunAsync(email, password, _dateTimeProvider.UtcNow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return context.Result!;
        }
        catch (DomainException)
        {
            // A failed attempt still mutates the account's lockout counters - persist
            // that even though the login itself is being rejected.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }
    }
}
