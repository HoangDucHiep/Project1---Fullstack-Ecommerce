using System.Security.Cryptography;
using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Infrastructure.Otp;

/// HDHiep - 10/11/2025
/// <summary>
/// Clean OTP service focused solely on OTP operations without business logic.
/// Handles OTP generation, storage, verification, and basic management.
/// </summary>
public class OtpService : IOtpService
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);
    private readonly ICacheService _redisService;

    public OtpService(ICacheService redisService)
    {
        _redisService = redisService;
    }

    /// <summary>
    /// Generates a cryptographically secure numeric OTP of specified length.
    /// </summary>
    /// <param name="length">Length of the OTP (default: 6)</param>
    /// <returns>Generated OTP string</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when length is less than or equal to zero</exception>
    public string GenerateOtp(int length = 6)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "OTP length must be greater than zero.");
        }

        const string digits = "0123456789";
        char[] otpChars = new char[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            for (int i = 0; i < length; i++)
            {
                otpChars[i] = digits[randomBytes[i] % digits.Length];
            }
        }

        return new string(otpChars);
    }

    /// <summary>
    /// Creates and stores a new OTP with the specified parameters.
    /// </summary>
    /// <param name="key">The unique cache key to associate with the OTP</param>
    /// <param name="otpLength">Length of the OTP to generate</param>
    /// <param name="ttl">Optional time-to-live for the OTP. If null, uses DefaultTtl (5 minutes)</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result containing the generated OTP if successful</returns>
    public async Task<Result<string>> CreateOtpAsync(string key, int otpLength, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        string otp = GenerateOtp(otpLength);

        var otpData = new OtpData(
            Otp: otp,
            FailedAttempts: 0,
            ResentCount: 0,
            ExpiresAt: DateTimeOffset.UtcNow.Add(ttl ?? DefaultTtl)
        );

        await _redisService.SetAsync(key, otpData, ttl ?? DefaultTtl, cancellationToken);
        return Result.Success(otp);
    }

    /// <summary>
    /// Regenerates a new OTP for an existing key, incrementing the resend count.
    /// </summary>
    /// <param name="key">The cache key associated with the existing OTP</param>
    /// <param name="ttl">Optional time-to-live for the new OTP. If null, uses DefaultTtl</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result containing the new OTP if successful</returns>
    public async Task<Result<string>> RegenerateOtpAsync(string key, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        Result<OtpData> result = await GetOtpAsync(key, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<string>(result.Error);
        }

        OtpData otpData = result.Value;

        // Generate new OTP and increment resend count
        string newOtp = GenerateOtp(otpData.Otp.Length);
        OtpData updatedOtpData = otpData with
        {
            Otp = newOtp,
            ResentCount = otpData.ResentCount + 1,
            ExpiresAt = DateTimeOffset.UtcNow.Add(ttl ?? DefaultTtl),
            FailedAttempts = 0 // Reset failed attempts on regeneration
        };

        await _redisService.SetAsync(key, updatedOtpData, ttl ?? DefaultTtl, cancellationToken);
        return Result.Success(newOtp);
    }

    /// <summary>
    /// Legacy method for backward compatibility - Creates OTP with additional parameters.
    /// </summary>
    public async Task<Result<string>> SetOtpAsync(string key, int otpLength, int resendDelayInSecond = 60, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        return await CreateOtpAsync(key, otpLength, ttl, cancellationToken);
    }

    /// <summary>
    /// Legacy method for backward compatibility - Regenerates OTP with additional parameters.
    /// </summary>
    public async Task<Result<string>> RegenerateOtpAsync(string key, int maxResends, int resendDelayInSecond, CancellationToken cancellationToken = default)
    {
        return await RegenerateOtpAsync(key, null, cancellationToken);
    }

    /// <summary>
    /// Retrieves OTP data from the cache by key.
    /// </summary>
    /// <param name="key">The cache key associated with the desired OTP</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result containing the OTP data if found, or a failure result if not found</returns>
    public async Task<Result<OtpData>> GetOtpAsync(string key, CancellationToken cancellationToken = default)
    {
        OtpData? otpData = await _redisService.GetAsync<OtpData>(key, cancellationToken);

        if (otpData is null)
        {
            return Result.Failure<OtpData>(OtpErrors.NotFound);
        }

        return Result.Success(otpData);
    }

    /// <summary>
    /// Verifies a provided OTP against the stored value with attempt tracking.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP to verify</param>
    /// <param name="otp">The OTP string provided by the user for verification</param>
    /// <param name="maxFailedAttempts">Maximum number of failed verification attempts allowed (default: 5)</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result indicating the verification outcome</returns>
    public async Task<Result> VerifyOtpAsync(string key, string otp, int maxFailedAttempts = 5, CancellationToken cancellationToken = default)
    {
        Result<OtpData> result = await GetOtpAsync(key, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        OtpData otpData = result.Value;

        // Check expiration
        if (otpData.ExpiresAt < DateTimeOffset.UtcNow)
        {
            await RemoveOtpAsync(key, cancellationToken);
            return Result.Failure(OtpErrors.Expired);
        }

        // Verify OTP
        if (otpData.Otp != otp)
        {
            OtpData updatedOtpData = otpData with { FailedAttempts = otpData.FailedAttempts + 1 };

            if (updatedOtpData.FailedAttempts >= maxFailedAttempts)
            {
                await RemoveOtpAsync(key, cancellationToken);
                return Result.Failure(OtpErrors.MaxAttemptsExceeded);
            }

            // Update OTP data with new attempt count, preserving remaining TTL
            TimeSpan remainingTime = updatedOtpData.ExpiresAt - DateTimeOffset.UtcNow;
            if (remainingTime > TimeSpan.Zero)
            {
                await _redisService.SetAsync(key, updatedOtpData, remainingTime, cancellationToken);
            }

            return Result.Failure(OtpErrors.Invalid(maxFailedAttempts - updatedOtpData.FailedAttempts));
        }

        // OTP is valid - remove it to prevent reuse
        await RemoveOtpAsync(key, cancellationToken);
        return Result.Success();
    }

    /// <summary>
    /// Removes an OTP from the cache.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result indicating success</returns>
    public async Task<Result> RemoveOtpAsync(string key, CancellationToken cancellationToken = default)
    {
        await _redisService.RemoveAsync(key, cancellationToken);
        return Result.Success();
    }

    /// <summary>
    /// Gets the number of resend attempts left for the given OTP.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="maxResends">Maximum allowed resend attempts</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result containing the number of resend attempts left</returns>
    public async Task<Result<int>> GetResendLeft(string key, int maxResends, CancellationToken cancellationToken = default)
    {
        Result<OtpData> result = await GetOtpAsync(key, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<int>(result.Error);
        }

        return Result.Success(Math.Max(0, maxResends - result.Value.ResentCount));
    }

    /// <summary>
    /// Checks if an OTP exists and is still valid (not expired).
    /// </summary>
    /// <param name="key">The cache key associated with the OTP</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation</param>
    /// <returns>A Result indicating whether the OTP exists and is valid</returns>
    public async Task<Result<bool>> IsOtpValidAsync(string key, CancellationToken cancellationToken = default)
    {
        Result<OtpData> result = await GetOtpAsync(key, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Success(false);
        }

        bool isValid = result.Value.ExpiresAt > DateTimeOffset.UtcNow;
        if (!isValid)
        {
            await RemoveOtpAsync(key, cancellationToken);
        }

        return Result.Success(isValid);
    }
}
