using System.Buffers;
using System.Text.Json;
using ECommerceBackend.Application.Abstracts.Caching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace ECommerceBackend.Infrastructure.Caching;

/// HDHiep - 09/24/2025
/// <summary>
/// Provides a caching service implementation using <see cref="IDistributedCache"/>.
/// This service allows storing, retrieving, and removing objects in distributed cache,
/// with support for JSON serialization/deserialization.
/// </summary>
public sealed class RedisService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisService"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache implementation to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cache"/> is null.</exception>
    public RedisService(IDistributedCache cache, IDatabase database)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _database = database ?? throw new ArgumentNullException(nameof(database));
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
        return _cache.SetAsync(key, bytes, RedisOptions.Create(expiration), cancellationToken);
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
    /// Gets the Time To Live (TTL) of a Redis key in seconds.
    /// </summary>
    /// <param name="key">The unique cache key.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>TTL in seconds. Returns -1 if key exists but has no expiration, -2 if key doesn't exist.</returns>
    public async Task<int> GetTTLAsync(string key, CancellationToken cancellationToken = default)
    {

        // First check if key exists
        bool keyExists = await _database.KeyExistsAsync(key);
        if (!keyExists)
        {
            return -2; // Key doesn't exist
        }

        // Get TTL for existing key
        TimeSpan? ttl = await _database.KeyTimeToLiveAsync(key);

        // If TTL is null, the key exists but has no expiration (persistent)
        if (ttl is null)
        {
            return -1; // Key exists but has no expiration
        }

        // Return TTL in seconds
        return (int)ttl.Value.TotalSeconds;
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

