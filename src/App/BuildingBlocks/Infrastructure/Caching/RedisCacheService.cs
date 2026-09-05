using System.Text.Json;
using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Infrastructure.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace App.BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Cache-Aside implementation of <see cref="ICacheService"/> backed by Redis.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly string _instanceName;

    public RedisCacheService(IConnectionMultiplexer multiplexer, IOptions<RedisOptions> options)
    {
        _multiplexer = multiplexer;
        _instanceName = options.Value.InstanceName;
    }

    private IDatabase Database => _multiplexer.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await Database.StringGetAsync(BuildKey(key));
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>((string)value!);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) =>
        Database.StringSetAsync(BuildKey(key), JsonSerializer.Serialize(value), expiration);

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan expiration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var created = await factory(cancellationToken);
        if (created is not null)
            await SetAsync(key, created, expiration, cancellationToken);

        return created;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        Database.KeyDeleteAsync(BuildKey(key));

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var pattern = $"{BuildKey(prefix)}*";
        foreach (var endpoint in _multiplexer.GetEndPoints())
        {
            var server = _multiplexer.GetServer(endpoint);
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                await Database.KeyDeleteAsync(key);
            }
        }
    }

    private string BuildKey(string key) => $"{_instanceName}{key}";
}
