using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Otp;


/// <summary>
/// Core OTP data stored in cache
/// </summary>
public record OtpData(
    string Otp,
    int FailedAttempts,
    int ResentCount,
    DateTimeOffset ExpiresAt
);


/// HDHiep - 10/11/2025
/// <summary>
/// Service for OTP generation, storage, and verification
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates a numeric OTP of specified length
    /// </summary>
    /// <param name="length">Length of the OTP</param>
    /// <returns></returns>
    string GenerateOtp(int length = 6);

    /// <summary>
    /// Stores the OTP in cache with an optional TTL
    /// </summary>
    /// <param name="key">The key associated with the OTP</param>
    /// <param name="otp">The OTP to store</param>
    /// <param name="ttl">Optional time-to-live for the OTP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<Result> SetOtpAsync(string key, string otp, TimeSpan? ttl = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the OTP data from cache
    /// </summary>
    /// <param name="key">The key associated with the OTP</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<OtpData>> GetOtpAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the provided OTP against the stored one, with a limit on failed attempts
    /// </summary>
    /// <param name="key">The key associated with the OTP</param>
    /// <param name="otp">The OTP to verify</param>
    /// <param name="maxFailedAttempts">Maximum allowed failed attempts before invalidating the OTP</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success or failure of the verification.
    /// If the OTP is invalid or expired, returns a failure result with appropriate error.
    /// </returns>
    Task<Result> VerifyOtpAsync(string key, string otp, int maxFailedAttempts = 5, CancellationToken cancellationToken = default);


    /// <summary>
    /// Regenerates and resends the OTP, with a limit on the number of resends
    /// </summary>
    /// <param name="key">The key associated with the OTP</param>
    /// <param name="maxResends">Maximum allowed resend attempts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// A <see cref="Result{string}"/> containing the new OTP if successful.
    /// </returns>
    Task<Result<string>> RegenerateOtpAsync(string key, int maxResends = 3, CancellationToken cancellationToken = default);


    /// <summary>
    /// Removes the OTP from cache
    /// </summary>
    /// <param name="key">The key associated with the OTP</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> RemoveOtpAsync(string key, CancellationToken cancellationToken = default);
}
