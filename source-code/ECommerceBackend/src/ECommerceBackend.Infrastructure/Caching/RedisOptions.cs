using Microsoft.Extensions.Caching.Distributed;

namespace ECommerceBackend.Infrastructure.Caching;

/// HDHiep - 09/24/2025
/// <summary>
/// Helper factory for creating <see cref="DistributedCacheEntryOptions"/> with sensible defaults.
/// </summary>
public static class RedisOptions
{
    /// <summary>
    /// Gets the default expiration policy (absolute 2 minutes).
    /// Use this when you want a standard expiration time.
    /// </summary>
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    /// <summary>
    /// Gets cache options for persistent keys (no expiration).
    /// </summary>
    public static DistributedCacheEntryOptions NeverExpires => new();

    /// <summary>
    /// Creates cache entry options with the given absolute expiration.
    /// </summary>
    /// <param name="expiration">
    /// Absolute expiration relative to now. 
    /// If null, the key will be persistent (no expiration).
    /// </param>
    /// <returns>The configured <see cref="DistributedCacheEntryOptions"/>.</returns>
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : NeverExpires;

    /// <summary>
    /// Creates cache entry options with custom absolute expiration time.
    /// </summary>
    /// <param name="minutes">
    /// Expiration time in minutes.
    /// If null, the default expiration of 2 minutes will be used.
    /// </param>
    /// <returns>The configured <see cref="DistributedCacheEntryOptions"/>.</returns>
    public static DistributedCacheEntryOptions CreateWithMinutes(int? minutes) =>
        minutes is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes.Value) }
            : DefaultExpiration;

    /// <summary>
    /// Creates cache entry options with custom absolute expiration time.
    /// </summary>
    /// <param name="seconds">
    /// Expiration time in seconds.
    /// If null, the default expiration policy will be used (2 minutes).
    /// </param>
    /// <returns>The configured <see cref="DistributedCacheEntryOptions"/>.</returns>
    public static DistributedCacheEntryOptions CreateWithSeconds(int? seconds) =>
        seconds is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds.Value) }
            : DefaultExpiration;
}
