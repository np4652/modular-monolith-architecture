namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Cache-Aside abstraction over the distributed cache. Application code must depend
/// on this rather than a Redis client directly.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);

    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan expiration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}
