namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Constants for user login with OTP process.
/// Contains cache keys, rate limiting rules, and timing configurations.
/// </summary>
internal static class LoginUserKeyConstants
{
    #region Cache Key Prefixes

    /// <summary>
    /// Rate limiter key prefix for user login OTP requests.
    /// Pattern: "ratelimit:user:login:{identifier}"
    /// </summary>
    public const string USER_LOGIN_OTP_RATE_LIMITER_PREFIX = "ratelimit:user:login:";

    /// <summary>
    /// OTP storage key prefix for login.
    /// Pattern: "otp:user:login:{identifier}"
    /// </summary>
    public const string USER_LOGIN_OTP_KEY = "otp:user:login:";

    /// <summary>
    /// Login session key prefix.
    /// Pattern: "session:user:login:{identifier}"
    /// </summary>
    public const string USER_LOGIN_SESSION_KEY = "session:user:login:";

    /// <summary>
    /// Resend delay lock key prefix for login.
    /// Pattern: "delay:user:login:{identifier}"
    /// </summary>
    public const string USER_LOGIN_RESEND_DELAY_KEY = "delay:user:login:";

    #endregion

    #region OTP Configuration

    /// <summary>
    /// Length of the generated OTP for login.
    /// </summary>
    public const int OTP_LENGTH = 6;

    /// <summary>
    /// Individual OTP lifetime in minutes for login.
    /// Each OTP is valid for this duration from creation/regeneration.
    /// </summary>
    public const int OTP_LIFE_TIME_IN_MINUTE = 5;

    /// <summary>
    /// Maximum number of OTP verification attempts before invalidating the OTP.
    /// </summary>
    public const int OTP_MAX_VERIFICATION_ATTEMPTS = 5;

    /// <summary>
    /// Maximum number of OTP resend attempts allowed per login session.
    /// </summary>
    public const int OTP_MAX_RESEND = 10;

    #endregion

    #region Rate Limiting Configuration

    /// <summary>
    /// Delay in seconds between OTP send requests for login.
    /// User must wait this long before requesting another OTP.
    /// </summary>
    public const int OTP_SEND_DELAY_IN_SECOND = 5;

    /// <summary>
    /// Delay in seconds between OTP resend requests for login.
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
    /// Login session lifetime in minutes.
    /// The entire login process must be completed within this time.
    /// </summary>
    public const int LOGIN_SESSION_LIFE_TIME_IN_MINUTE = 10;

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the rate limiter key for a specific identifier.
    /// </summary>
    /// <param name="identifier">The identifier (phone number or email)</param>
    /// <returns>Rate limiter cache key</returns>
    public static string GetRateLimiterKey(string identifier) =>
        $"{USER_LOGIN_OTP_RATE_LIMITER_PREFIX}{identifier}";

    /// <summary>
    /// Gets the OTP storage key for a specific identifier.
    /// </summary>
    /// <param name="identifier">The identifier (phone number or email)</param>
    /// <returns>OTP storage cache key</returns>
    public static string GetOtpKey(string identifier) =>
        $"{USER_LOGIN_OTP_KEY}{identifier}";

    /// <summary>
    /// Gets the login session key for a specific identifier.
    /// </summary>
    /// <param name="identifier">The identifier (phone number or email)</param>
    /// <returns>Session storage cache key</returns>
    public static string GetSessionKey(string identifier) =>
        $"{USER_LOGIN_SESSION_KEY}{identifier}";

    /// <summary>
    /// Gets the resend delay lock key for a specific identifier.
    /// </summary>
    /// <param name="identifier">The identifier (phone number or email)</param>
    /// <returns>Resend delay lock cache key</returns>
    public static string GetResendDelayKey(string identifier) =>
        $"{USER_LOGIN_RESEND_DELAY_KEY}{identifier}";

    #endregion
}
