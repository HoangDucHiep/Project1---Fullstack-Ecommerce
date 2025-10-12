using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Infrastructure.RateLimiter;


/// HDHiep - 10/11/2025
/// <summary>
/// Service to handle rate limiting functionality.
/// </summary>
public class RateLimiterService : IRateLimiterService
{
    private readonly ICacheService _redisService;

    public RateLimiterService(ICacheService redisService)
    {
        _redisService = redisService;
    }

    /// <summary>
    /// Checks if the operation identified by 'key' is within the allowed rate limit.
    /// </summary>
    /// <param name="key">The unique identifier for the operation.</param>
    /// <param name="maxAttempts">The maximum number of allowed attempts.</param>
    /// <param name="windowSeconds">The time window in seconds for the rate limit.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> CheckRateLimitAsync(string key, int maxAttempts, int windowSeconds = 900, CancellationToken cancellationToken = default)
    {
        Result<int> countResult = await GetAttemptCountAsync(key, cancellationToken);

        if (countResult.IsFailure)
        {
            return Result.Failure(countResult.Error);
        }

        if (countResult.Value >= maxAttempts)
        {
            int waitMinutes = windowSeconds / 60;
            return Result.Failure(RateLimiterErrors.TooManyRequests(waitMinutes));
        }

        return Result.Success();
    }

    /// <summary>
    /// Gets the current attempt count for the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the operation.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<int>> GetAttemptCountAsync(string key, CancellationToken cancellationToken = default)
    {
        int? count = await _redisService.GetAsync<int?>(key, cancellationToken);
        return Result.Success(count ?? 0);
    }


    /// <summary>
    /// Increments the attempt count for the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the operation.</param>
    /// <param name="windowSeconds">The time window in seconds for the rate limit.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> IncrementAsync(string key, int windowSeconds = 900, CancellationToken cancellationToken = default)
    {
        Result<int> countResult = await GetAttemptCountAsync(key, cancellationToken);

        if (countResult.IsFailure)
        {
            return Result.Failure(countResult.Error);
        }

        int currentCount = countResult.IsSuccess ? countResult.Value : 0;

        await _redisService.SetAsync(key, currentCount + 1, TimeSpan.FromSeconds(windowSeconds), cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Checks if the resource identified by 'key' is currently locked.
    /// </summary>
    /// <param name="key">The unique identifier for the operation.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<bool>> IsLockedAsync(string key, CancellationToken cancellationToken = default)
    {
        bool? isLocked = await _redisService.GetAsync<bool?>(key, cancellationToken);
        return Result.Success(isLocked == true);
    }

    /// <summary>
    /// Locks the resource identified by 'key' for a specified duration.
    /// </summary>
    /// <param name="key">The unique identifier for the operation.</param>
    /// <param name="durationSeconds">The duration in seconds for which the resource should be locked.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> LockAsync(string key, int durationSeconds, CancellationToken cancellationToken = default)
    {
        await _redisService.SetAsync(key, true, TimeSpan.FromSeconds(durationSeconds), cancellationToken);
        return Result.Success();
    }


    /// <summary>
    /// Removes the rate limit or lock associated with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the operation.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _redisService.RemoveAsync(key, cancellationToken);
        return Result.Success();
    }
}
