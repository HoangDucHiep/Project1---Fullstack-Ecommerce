using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Otp;

/// HDHiep - 10/11/2025
/// <summary>
/// Service for OTP generation, storage, and verification without business logic.
/// Focuses purely on OTP operations.
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates a cryptographically secure numeric OTP of specified length.
    /// </summary>
    /// <param name="length">Length of the OTP (default: 6)</param>
    /// <returns>Generated OTP string</returns>
    string GenerateOtp(int length = 6);

    /// <summary>
    /// Creates and stores a new OTP.
    /// </summary>
    /// <param name="key">The unique cache key to associate with the OTP</param>
    /// <param name="otpLength">Length of the OTP to generate</param>
    /// <param name="ttl">Optional time-to-live for the OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the generated OTP</returns>
    Task<Result<string>> CreateOtpAsync(string key, int otpLength, TimeSpan? ttl = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Regenerates a new OTP for an existing key, incrementing the resend count.
    /// </summary>
    /// <param name="key">The cache key associated with the existing OTP</param>
    /// <param name="ttl">Optional time-to-live for the new OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the new OTP</returns>
    Task<Result<string>> RegenerateOtpAsync(string key, TimeSpan? ttl = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Legacy method for backward compatibility - Creates OTP with additional parameters.
    /// Use CreateOtpAsync for new implementations.
    /// </summary>
    /// <param name="key">The unique cache key to associate with the OTP</param>
    /// <param name="otpLength">Length of the OTP to generate</param>
    /// <param name="resendDelayInSecond">Resend delay (ignored in this implementation)</param>
    /// <param name="ttl">Optional time-to-live for the OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the generated OTP</returns>
    Task<Result<string>> SetOtpAsync(string key, int otpLength, int resendDelayInSecond = 60, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        // Default implementation delegates to CreateOtpAsync
        return CreateOtpAsync(key, otpLength, ttl, cancellationToken);
    }

    /// <summary>
    /// Legacy method for backward compatibility - Regenerates OTP with additional parameters.  
    /// Use RegenerateOtpAsync for new implementations.
    /// </summary>
    /// <param name="key">The cache key associated with the existing OTP</param>
    /// <param name="maxResends">Maximum resends (ignored - check in business logic)</param>
    /// <param name="resendDelayInSecond">Resend delay (ignored in this implementation)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the new OTP</returns>
    Task<Result<string>> RegenerateOtpAsync(string key, int maxResends, int resendDelayInSecond, CancellationToken cancellationToken = default)
    {
        // Default implementation delegates to RegenerateOtpAsync
        return RegenerateOtpAsync(key, null, cancellationToken);
    }

    /// <summary>
    /// Retrieves the OTP data from cache.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing OTP data</returns>
    Task<Result<OtpData>> GetOtpAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the provided OTP against the stored one.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="otp">The OTP to verify</param>
    /// <param name="maxFailedAttempts">Maximum allowed failed attempts (default: 5)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating verification success or failure</returns>
    Task<Result> VerifyOtpAsync(string key, string otp, int maxFailedAttempts = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the number of resend attempts left.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="maxResends">Maximum allowed resends</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing resend attempts left</returns>
    Task<Result<int>> GetResendLeft(string key, int maxResends, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the OTP from cache.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success</returns>
    Task<Result> RemoveOtpAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an OTP exists and is still valid (not expired).
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating whether the OTP exists and is valid</returns>
    Task<Result<bool>> IsOtpValidAsync(string key, CancellationToken cancellationToken = default);
}
