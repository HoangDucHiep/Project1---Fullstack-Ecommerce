namespace ECommerceBackend.Application.Authentication.RegisterUserWithOtp;

/// <summary>
/// Constants for user registration with OTP process.
/// Contains cache keys, rate limiting rules, and timing configurations.
/// </summary>
internal static class RegisterUserKeyConstants
{
    #region Cache Key Prefixes

    /// <summary>
    /// Rate limiter key prefix for user registration OTP requests.
    /// Pattern: "ratelimit:user:register:{phoneNumber}"
    /// </summary>
    public const string USER_REGISTER_OTP_RATE_LIMITER_PREFIX = "ratelimit:user:register:";

    /// <summary>
    /// OTP storage key prefix.
    /// Pattern: "otp:user:register:{phoneNumber}"
    /// </summary>
    public const string USER_REGISTER_OTP_KEY = "otp:user:register:";

    /// <summary>
    /// Registration session key prefix.
    /// Pattern: "session:user:register:{phoneNumber}"
    /// </summary>
    public const string USER_REGISTER_SESSION_KEY = "session:user:register:";

    /// <summary>
    /// Resend delay lock key prefix.
    /// Pattern: "delay:user:register:{phoneNumber}"
    /// </summary>
    public const string USER_REGISTER_RESEND_DELAY_KEY = "delay:user:register:";

    #endregion

    #region OTP Configuration

    /// <summary>
    /// Length of the generated OTP.
    /// </summary>
    public const int OTP_LENGTH = 6;

    /// <summary>
    /// Individual OTP lifetime in minutes.
    /// Each OTP is valid for this duration from creation/regeneration.
    /// </summary>
    public const int OTP_LIFE_TIME_IN_MINUTE = 5;

    /// <summary>
    /// Maximum number of OTP verification attempts before invalidating the OTP.
    /// </summary>
    public const int OTP_MAX_VERIFICATION_ATTEMPTS = 5;

    /// <summary>
    /// Maximum number of OTP resend attempts allowed per session.
    /// </summary>
    public const int OTP_MAX_RESEND = 10;

    #endregion

    #region Rate Limiting Configuration

    /// <summary>
    /// Delay in seconds between OTP send requests.
    /// User must wait this long before requesting another OTP.
    /// </summary>
    public const int OTP_SEND_DELAY_IN_SECOND = 5;

    /// <summary>
    /// Delay in seconds between OTP resend requests.
    /// User must wait this long before requesting another OTP.
    /// </summary>
    public const int OTP_RESEND_DELAY_IN_SECOND = 60;

    /// <summary>
    /// Maximum number of OTP creation attempts within the rate limit window.
    /// </summary>
    public const int RATE_LIMIT_MAX_ATTEMPTS = 5;

    /// <summary>
    /// Rate limiting window in seconds (15 minutes).
    /// Rate limit counter resets after this duration.
    /// </summary>
    public const int RATE_LIMIT_WINDOW_SECONDS = 900; // 15 minutes

    /// <summary>
    /// Block duration in minutes when user reaches maximum resend limit.
    /// User must wait this long after exhausting all resend attempts.
    /// </summary>
    public const int RESEND_LIMIT_BLOCK_TIME_IN_MINUTE = 30;

    #endregion

    #region Session Configuration

    /// <summary>
    /// Registration session lifetime in minutes.
    /// The entire registration process must be completed within this time.
    /// </summary>
    public const int REGISTRATION_SESSION_LIFE_TIME_IN_MINUTE = 10;

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the rate limiter key for a specific phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Rate limiter cache key</returns>
    public static string GetRateLimiterKey(string phoneNumber) =>
        $"{USER_REGISTER_OTP_RATE_LIMITER_PREFIX}{phoneNumber}";

    /// <summary>
    /// Gets the OTP storage key for a specific phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>OTP storage cache key</returns>
    public static string GetOtpKey(string phoneNumber) =>
        $"{USER_REGISTER_OTP_KEY}{phoneNumber}";

    /// <summary>
    /// Gets the registration session key for a specific phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Session storage cache key</returns>
    public static string GetSessionKey(string phoneNumber) =>
        $"{USER_REGISTER_SESSION_KEY}{phoneNumber}";

    /// <summary>
    /// Gets the resend delay lock key for a specific phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Resend delay lock cache key</returns>
    public static string GetResendDelayKey(string phoneNumber) =>
        $"{USER_REGISTER_RESEND_DELAY_KEY}{phoneNumber}";

    #endregion
}
