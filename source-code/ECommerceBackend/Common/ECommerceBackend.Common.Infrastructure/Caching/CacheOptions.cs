using Microsoft.Extensions.Caching.Distributed;

namespace ECommerceBackend.Common.Infrastructure.Caching;

/// <summary>
/// Helper factory for creating <see cref="DistributedCacheEntryOptions"/> with sensible defaults.
/// </summary>
public static class CacheOptions
{
    /// <summary>
    /// Gets the default expiration policy (absolute 2 minutes).
    /// </summary>
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    /// <summary>
    /// Creates cache entry options with the given absolute expiration or falls back to <see cref="DefaultExpiration"/>.
    /// </summary>
    /// <param name="expiration">Absolute expiration relative to now.</param>
    /// <returns>The configured <see cref="DistributedCacheEntryOptions"/>.</returns>
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null ?
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } :
            DefaultExpiration;
}
