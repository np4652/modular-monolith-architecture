using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Domain.Events;

namespace App.Modules.UserManagement.Application.EventHandlers;

public sealed class UserLockedOutEventHandler : IDomainEventHandler<UserLockedOutEvent>
{
    private readonly ILogger<UserLockedOutEventHandler> _logger;

    public UserLockedOutEventHandler(ILogger<UserLockedOutEventHandler> logger) => _logger = logger;

    public Task HandleAsync(UserLockedOutEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "User {UserId} locked out until {LockedUntilUtc} after repeated failed login attempts",
            domainEvent.UserId, domainEvent.LockedUntilUtc);

        return Task.CompletedTask;
    }
}
