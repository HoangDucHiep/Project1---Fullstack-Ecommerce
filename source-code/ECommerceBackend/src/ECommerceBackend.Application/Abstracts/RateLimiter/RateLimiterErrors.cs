using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.RateLimiter;

/// HDHiep - 10/11/2025
/// <summary>
/// Common rate limiter-related errors
/// </summary>
public static class RateLimiterErrors
{
    public static Error TooManyRequests(int waitMinutes) => Error.Validation(
        "RateLimit.Exceeded",
        $"Too many requests. Please wait {waitMinutes} minutes.");

    public static Error Locked(int waitMinutes) => Error.Validation(
        "RateLimit.Locked",
        $"Resource locked. Please wait {waitMinutes} minutes.");

    public static Error LockNotFound() => Error.NotFound(
        "RateLimit.LockNotFound",
        "Rate limit lock not found or already expired");
}
