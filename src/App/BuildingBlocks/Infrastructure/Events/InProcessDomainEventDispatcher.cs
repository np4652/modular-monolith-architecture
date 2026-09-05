using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Abstractions;

namespace App.BuildingBlocks.Infrastructure.Events;

/// <summary>
/// Resolves and invokes every <see cref="IDomainEventHandler{TEvent}"/> registered for
/// each raised event's concrete type. Deliberately reflection-based and dependency-free
/// rather than pulling in a mediator library, per the "no MediatR/CQRS" constraint.
/// </summary>
public sealed class InProcessDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InProcessDomainEventDispatcher> _logger;

    public InProcessDomainEventDispatcher(IServiceProvider serviceProvider, ILogger<InProcessDomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null) continue;

                _logger.LogDebug(
                    "Dispatching {DomainEvent} ({EventId}) to {Handler}",
                    domainEvent.GetType().Name, domainEvent.EventId, handler.GetType().Name);

                var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;
                await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
            }
        }
    }
}
