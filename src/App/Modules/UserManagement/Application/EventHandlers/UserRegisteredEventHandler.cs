using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Domain.Events;

namespace App.Modules.UserManagement.Application.EventHandlers;

/// <summary>
/// Reacts to a new registration in-process, and relays it as an integration event
/// so a future module (e.g. Notification, sending a welcome email) can subscribe
/// without UserManagement ever knowing that module exists.
/// </summary>
public sealed class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;
    private readonly IEventBus _eventBus;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} registered with email {Email}", domainEvent.UserId, domainEvent.Email);

        await _eventBus.PublishAsync(domainEvent, cancellationToken);
    }
}
