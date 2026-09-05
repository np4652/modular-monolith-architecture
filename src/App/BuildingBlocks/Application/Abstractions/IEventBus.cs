namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Outbound asynchronous messaging abstraction (RabbitMQ in Infrastructure). Used when
/// a domain event must also be observable outside this process' in-memory dispatcher -
/// e.g. by a future module or external consumer. Application code never depends on
/// RabbitMQ types directly.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : class;
}
