using System.Text;
using System.Text.Json;
using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Infrastructure.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace App.BuildingBlocks.Infrastructure.Messaging;

/// <summary>
/// Publishes integration events onto a durable topic exchange for asynchronous,
/// out-of-process consumption. Application code depends only on <see cref="IEventBus"/>.
/// </summary>
public sealed class RabbitMqEventBus : IEventBus, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqEventBus(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventBus> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : class
    {
        try
        {
            var channel = await GetChannelAsync(cancellationToken);
            var routingKey = typeof(TIntegrationEvent).Name;
            var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent);

            var properties = new BasicProperties { Persistent = true, ContentType = "application/json" };

            await channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Published integration event {EventType}", routingKey);
        }
        catch (Exception ex)
        {
            // Messaging is a best-effort side channel here (no outbox yet); a broker
            // outage must never fail the use case that already committed to the database.
            _logger.LogError(ex, "Failed to publish integration event {EventType}", typeof(TIntegrationEvent).Name);
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_channel is { IsOpen: true })
                return _channel;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await _channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, durable: true,
                cancellationToken: cancellationToken);

            return _channel;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
        _connectionLock.Dispose();
    }
}
