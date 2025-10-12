using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.RateLimiter;


/// HDHiep - 10/11/2025
/// <summary>
/// Service for distributed rate limiting using Redis
/// </summary>
public interface IRateLimiterService
{
    /// <summary>
    /// Checks if operation is within rate limit
    /// </summary>
    Task<Result> CheckRateLimitAsync(string key, int maxAttempts, int windowSeconds = 900, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments rate limit counter
    /// </summary>
    Task<Result> IncrementAsync(string key, int windowSeconds = 900, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current attempt count
    /// </summary>
    Task<Result<int>> GetAttemptCountAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks a resource for specified duration
    /// </summary>
    Task<Result> LockAsync(string key, int durationSeconds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if resource is locked
    /// </summary>
    Task<Result<bool>> IsLockedAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes rate limit or lock
    /// </summary>
    Task<Result> RemoveAsync(string key, CancellationToken cancellationToken = default);
}
