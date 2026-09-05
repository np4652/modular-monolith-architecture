namespace App.BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Centralized expiration categories so modules never invent arbitrary cache lifetimes.
/// </summary>
public static class CacheExpiration
{
    public static readonly TimeSpan Short = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan Medium = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan Long = TimeSpan.FromHours(6);
}
