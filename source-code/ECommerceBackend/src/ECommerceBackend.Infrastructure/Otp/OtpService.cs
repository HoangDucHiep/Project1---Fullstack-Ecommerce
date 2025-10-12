using System.Security.Cryptography;
using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Infrastructure.Otp;

/// HDHiep - 10/11/2025
/// <summary>
/// Implementation of OTP (One-Time Password) service that provides secure OTP generation,
/// storage, verification, and management capabilities using distributed caching.
/// </summary>
/// <remarks>
/// This service handles the complete OTP lifecycle including:
/// - Cryptographically secure OTP generation
/// - Redis-based storage with TTL management
/// - Verification with failed attempt tracking
/// - Regeneration with resend limits
/// - Automatic cleanup and expiration handling
public class OtpService : IOtpService
{
    /// <summary>
    /// The cache service used for storing and retrieving OTP data.
    /// </summary>
    private readonly ICacheService _redisService;

    /// <summary>
    /// The default time-to-live for OTPs when no custom TTL is specified.
    /// Set to 5 minutes for security best practices.
    /// </summary>
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="OtpService"/> class.
    /// </summary>
    /// <param name="redisService">The cache service implementation for OTP storage.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="redisService"/> is null.</exception>
    public OtpService(ICacheService redisService)
    {
        _redisService = redisService;
    }


    /// <summary>
    /// Generates a cryptographically secure numeric OTP of specified length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
    /// Stores an OTP in the cache with associated metadata and expiration settings.
    /// </summary>
    /// <param name="key">The unique cache key to associate with the OTP.</param>
    /// <param name="otp">The OTP string to store.</param>
    /// <param name="ttl">
    /// Optional time-to-live for the OTP. If null, uses <see cref="DefaultTtl"/> (5 minutes).
    /// </param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success or failure of the storage operation.
    /// </returns>
    /// <remarks>
    /// Creates an <see cref="OtpData"/> object with:
    /// - Zero failed attempts
    /// - Zero resent count
    /// - Expiration time based on the provided or default TTL
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = await SetOtpAsync("user123", "Xk9pLm==", TimeSpan.FromMinutes(10));
    /// if (result.IsSuccess)
    /// {
    ///     // OTP stored successfully
    /// }
    /// </code>
    /// </example>
    public async Task<Result> SetOtpAsync(string key, string otp, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {

        var otpData = new OtpData(
            Otp: otp,
            FailedAttempts: 0,
            ResentCount: 0,
            ExpiresAt: DateTimeOffset.UtcNow.Add(ttl ?? DefaultTtl)
        );

        await _redisService.SetAsync(key, otpData, ttl ?? DefaultTtl, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Retrieves OTP data from the cache by key.
    /// </summary>
    /// <param name="key">The cache key associated with the desired OTP.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Result{OtpData}"/> containing the OTP data if found,
    /// or a failure result with <see cref="OtpErrors.NotFound"/> if not found.
    /// </returns>
    /// <remarks>
    /// This method does not perform expiration checks - it only retrieves data from cache.
    /// Expiration validation should be done in consuming methods like <see cref="VerifyOtpAsync"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = await GetOtpAsync("user123");
    /// if (result.IsSuccess)
    /// {
    ///     var otpData = result.Value;
    ///     Console.WriteLine($"OTP expires at: {otpData.ExpiresAt}");
    /// }
    /// </code>
    /// </example>
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
    /// Verifies a provided OTP against the stored value with comprehensive validation and attempt tracking.
    /// </summary>
    /// <param name="key">The cache key associated with the OTP to verify.</param>
    /// <param name="otp">The OTP string provided by the user for verification.</param>
    /// <param name="maxFailedAttempts">
    /// Maximum number of failed verification attempts allowed before invalidating the OTP.
    /// Default is 5 attempts.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating the verification outcome:
    /// - Success: OTP is valid and matches
    /// - Failure: OTP not found, expired, invalid, or max attempts exceeded
    /// </returns>
    /// <remarks>
    /// <para>Verification process:</para>
    /// <list type="number">
    /// <item>Retrieves OTP data from cache</item>
    /// <item>Checks if OTP has expired (removes if expired)</item>
    /// <item>Compares provided OTP with stored value</item>
    /// <item>Increments failed attempts on mismatch</item>
    /// <item>Removes OTP if max attempts exceeded</item>
    /// <item>Updates cache with new attempt count if under limit</item>
    /// </list>
    /// 
    /// <para>Possible error results:</para>
    /// <list type="bullet">
    /// <item><see cref="OtpErrors.NotFound"/>: No OTP found for the key</item>
    /// <item><see cref="OtpErrors.Expired"/>: OTP has passed its expiration time</item>
    /// <item><see cref="OtpErrors.Invalid"/>: OTP doesn't match (under attempt limit)</item>
    /// <item><see cref="OtpErrors.MaxAttemptsExceeded"/>: Too many failed attempts</item>
    /// </list>
    /// </remarks>
    public async Task<Result> VerifyOtpAsync(string key, string otp, int maxFailedAttempts = 5, CancellationToken cancellationToken = default)
    {
        Result<OtpData> result = await GetOtpAsync(key, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        OtpData otpData = result.Value;


        // If the OTP is already expired
        if (otpData.ExpiresAt < DateTimeOffset.UtcNow)
        {
            await RemoveOtpAsync(key, cancellationToken);
            return Result.Failure(OtpErrors.Expired);
        }

        // If the OTP matches
        if (otpData.Otp != otp)
        {
            // First we increment the failed attempts
            OtpData updatedOptData = otpData with { FailedAttempts = otpData.FailedAttempts + 1 };

            // If exceeded max attempts, remove the OTP and return error
            if (updatedOptData.FailedAttempts >= maxFailedAttempts)
            {
                await RemoveOtpAsync(key, cancellationToken);
                return Result.Failure(OtpErrors.MaxAttemptsExceeded);
            }

            // Otherwise, update the OTP data in cache
            await _redisService.SetAsync(key, updatedOptData, updatedOptData.ExpiresAt - DateTimeOffset.UtcNow, cancellationToken);
            return Result.Failure(OtpErrors.Invalid(maxFailedAttempts - updatedOptData.FailedAttempts));
        }

        // If OTP is valid, done
        return Result.Success();
    }

    /// <summary>
    /// Regenerates a new OTP for the given key, with limits on the number of resends.
    /// </summary>      
    public async Task<Result<string>> RegenerateOtpAsync(string key, int maxResends = 3, CancellationToken cancellationToken = default)
    {
        Result<OtpData> result = await GetOtpAsync(key, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<string>(result.Error);
        }

        OtpData otpData = result.Value;

        // If the OTP session is already reached max resend attempts
        if (otpData.ResentCount >= maxResends)
        {
            return Result.Failure<string>(OtpErrors.ResendLimitReached);
        }

        // Generate a new OTP
        string newOtp = GenerateOtp(otpData.Otp.Length);
        OtpData updatedOtpData = otpData with
        {
            Otp = newOtp,
            ResentCount = otpData.ResentCount + 1,
            ExpiresAt = DateTimeOffset.UtcNow.Add(otpData.ExpiresAt - (DateTimeOffset.UtcNow - (otpData.ExpiresAt - otpData.ExpiresAt.AddMinutes(-5))))
        };

        await _redisService.SetAsync(key, updatedOtpData, updatedOtpData.ExpiresAt - DateTimeOffset.UtcNow, cancellationToken);

        return Result.Success(newOtp);
    }

    public async Task<Result> RemoveOtpAsync(string key, CancellationToken cancellationToken = default)
    {
        await _redisService.RemoveAsync(key, cancellationToken);
        return Result.Success();
    }
}
