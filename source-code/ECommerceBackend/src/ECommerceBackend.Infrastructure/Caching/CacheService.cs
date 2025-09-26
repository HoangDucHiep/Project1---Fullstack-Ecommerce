using System.Buffers;
using System.Text.Json;
using ECommerceBackend.Application.Abstracts.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerceBackend.Infrastructure.Caching;

/// HDHiep - 09/24/2025
/// <summary>
/// Provides a caching service implementation using <see cref="IDistributedCache"/>.
/// This service allows storing, retrieving, and removing objects in distributed cache,
/// with support for JSON serialization/deserialization.
/// </summary>
public sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheService"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache implementation to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cache"/> is null.</exception>
    public CacheService(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves a cached item by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached object.</typeparam>
    /// <param name="key">The unique cache key.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// The cached item if found; otherwise, <c>default</c>.
    /// </returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);
        return bytes is null ? default : Deserialize<T>(bytes);
    }

    /// <summary>
    /// Stores an item in the cache with the given key and optional expiration.
    /// </summary>
    /// <typeparam name="T">The type of the object to cache.</typeparam>
    /// <param name="key">The unique cache key.</param>
    /// <param name="value">The value to store in the cache.</param>
    /// <param name="expiration">Optional expiration time. If null, default cache policy is used.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);
        return _cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
    }

    /// <summary>
    /// Removes an item from the cache by its key.
    /// </summary>
    /// <param name="key">The unique cache key.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }

    /// <summary>
    /// Deserializes a byte array to a strongly-typed object using <see cref="JsonSerializer"/>.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    /// <param name="bytes">The byte array representing the serialized object.</param>
    /// <returns>The deserialized object.</returns>
    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    /// <summary>
    /// Serializes a strongly-typed object into a byte array using <see cref="JsonSerializer"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A byte array containing the JSON representation of the object.</returns>
    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }
}

