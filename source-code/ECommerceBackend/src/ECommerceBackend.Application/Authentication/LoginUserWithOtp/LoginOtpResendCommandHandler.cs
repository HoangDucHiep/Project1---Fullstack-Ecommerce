using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Application.Abstracts.Sms;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Handler for resending OTP during user login process.
/// Manages OTP regeneration with proper validation of session state,
/// resend limits, and rate limiting.
/// Returns the number of resend attempts remaining.
/// </summary>
internal sealed class LoginOtpResendCommandHandler : ICommandHandler<LoginOtpResendCommand, int>
{
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly ICacheService _cacheService;

    public LoginOtpResendCommandHandler(
        IOtpService otpService,
        ISmsService smsService,
        IRateLimiterService rateLimiterService,
        ICacheService cacheService)
    {
        _otpService = otpService;
        _smsService = smsService;
        _rateLimiterService = rateLimiterService;
        _cacheService = cacheService;
    }

    public async Task<Result<int>> Handle(LoginOtpResendCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate login session exists
        Result<LoginSession> sessionValidationResult = await ValidateLoginSessionAsync(request.Identifier, cancellationToken);
        if (sessionValidationResult.IsFailure)
        {
            return Result.Failure<int>(sessionValidationResult.Error);
        }

        LoginSession loginSession = sessionValidationResult.Value;

        // 2. Check resend delay
        Result resendDelayResult = await CheckResendDelayAsync(request.Identifier, cancellationToken);
        if (resendDelayResult.IsFailure)
        {
            return Result.Failure<int>(resendDelayResult.Error);
        }

        // 3. Validate OTP exists and check resend limits
        Result otpValidationResult = await ValidateOtpAndResendLimitsAsync(request.Identifier, cancellationToken);
        if (otpValidationResult.IsFailure)
        {
            return Result.Failure<int>(otpValidationResult.Error);
        }

        // 4. Regenerate OTP
        Result<string> newOtpResult = await RegenerateOtpAsync(request.Identifier, cancellationToken);
        if (newOtpResult.IsFailure)
        {
            return Result.Failure<int>(newOtpResult.Error);
        }

        // 5. Update session expiration
        await RefreshLoginSessionAsync(request.Identifier, loginSession, cancellationToken);

        // 6. Send OTP via SMS (only if phone number exists)
        if (!string.IsNullOrEmpty(loginSession.PhoneNumber))
        {
            Result smsResult = await SendOtpSmsAsync(loginSession.PhoneNumber, newOtpResult.Value, cancellationToken);
            if (smsResult.IsFailure)
            {
                // Cleanup on SMS failure
                await CleanupFailedResendAsync(request.Identifier, cancellationToken);
                return Result.Failure<int>(smsResult.Error);
            }
        }

        // 7. Apply resend delay
        await ApplyResendDelayAsync(request.Identifier, cancellationToken);

        // 8. Return remaining resend attempts
        Result<int> resendLeftResult = await GetRemainingResendAttemptsAsync(request.Identifier, cancellationToken);
        return resendLeftResult.IsSuccess
            ? Result.Success(resendLeftResult.Value)
            : Result.Failure<int>(resendLeftResult.Error);
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates that the login session exists and is valid.
    /// </summary>
    private async Task<Result<LoginSession>> ValidateLoginSessionAsync(string identifier, CancellationToken cancellationToken)
    {
        string sessionKey = LoginUserKeyConstants.GetSessionKey(identifier);
        LoginSession? session = await _cacheService.GetAsync<LoginSession>(sessionKey, cancellationToken);

        if (session is null)
        {
            return Result.Failure<LoginSession>(OtpErrors.SessionExpired);
        }

        return Result.Success(session);
    }

    /// <summary>
    /// Checks if the user is still within the resend delay period.
    /// </summary>
    private async Task<Result> CheckResendDelayAsync(string identifier, CancellationToken cancellationToken)
    {
        string resendDelayKey = LoginUserKeyConstants.GetResendDelayKey(identifier);

        Result<bool> isLocked = await _rateLimiterService.IsLockedAsync(resendDelayKey, cancellationToken);

        if (isLocked.IsSuccess && isLocked.Value)
        {
            Result<int> delayLeft = await _rateLimiterService.GetLockSecondsAliveLeft(resendDelayKey, cancellationToken);
            if (delayLeft.IsSuccess)
            {
                return Result.Failure(OtpErrors.OtpDelay(delayLeft.Value));
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Validates that OTP exists and user hasn't exceeded resend limits.
    /// </summary>
    private async Task<Result> ValidateOtpAndResendLimitsAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);

        // Check if OTP exists
        Result<OtpData> otpResult = await _otpService.GetOtpAsync(otpKey, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure(OtpErrors.SessionExpired);
        }

        // Check resend limit
        Result<int> resendLeft = await _otpService.GetResendLeft(
            otpKey,
            LoginUserKeyConstants.OTP_MAX_RESEND,
            cancellationToken);

        if (resendLeft.IsSuccess && resendLeft.Value <= 0)
        {
            double blockTimeSeconds = TimeSpan.FromMinutes(LoginUserKeyConstants.RESEND_LIMIT_BLOCK_TIME_IN_MINUTE).TotalSeconds;
            return Result.Failure(OtpErrors.ResendLimitReached((int)blockTimeSeconds));
        }

        return Result.Success();
    }

    /// <summary>
    /// Regenerates the OTP for the user.
    /// </summary>
    private async Task<Result<string>> RegenerateOtpAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);
        var otpTtl = TimeSpan.FromMinutes(LoginUserKeyConstants.OTP_LIFE_TIME_IN_MINUTE);

        return await _otpService.RegenerateOtpAsync(otpKey, otpTtl, cancellationToken);
    }

    /// <summary>
    /// Refreshes the login session expiration time.
    /// </summary>
    private async Task RefreshLoginSessionAsync(string identifier, LoginSession session, CancellationToken cancellationToken)
    {
        string sessionKey = LoginUserKeyConstants.GetSessionKey(identifier);
        var sessionTtl = TimeSpan.FromMinutes(LoginUserKeyConstants.LOGIN_SESSION_LIFE_TIME_IN_MINUTE);

        await _cacheService.SetAsync(sessionKey, session, sessionTtl, cancellationToken);
    }

    /// <summary>
    /// Sends the OTP to the user via SMS.
    /// </summary>
    private async Task<Result> SendOtpSmsAsync(string phoneNumber, string otp, CancellationToken cancellationToken)
    {
        return await _smsService.SendOtpAsync(phoneNumber, otp, cancellationToken);
    }

    /// <summary>
    /// Applies resend delay to prevent immediate consecutive requests.
    /// </summary>
    private async Task ApplyResendDelayAsync(string identifier, CancellationToken cancellationToken)
    {
        string resendDelayKey = LoginUserKeyConstants.GetResendDelayKey(identifier);

        await _rateLimiterService.LockAsync(
            resendDelayKey,
            LoginUserKeyConstants.OTP_RESEND_DELAY_IN_SECOND,
            cancellationToken);
    }

    /// <summary>
    /// Gets the remaining number of resend attempts.
    /// </summary>
    private async Task<Result<int>> GetRemainingResendAttemptsAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);

        return await _otpService.GetResendLeft(
            otpKey,
            LoginUserKeyConstants.OTP_MAX_RESEND,
            cancellationToken);
    }

    /// <summary>
    /// Cleans up data when SMS sending fails.
    /// </summary>
    private async Task CleanupFailedResendAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);
        string sessionKey = LoginUserKeyConstants.GetSessionKey(identifier);
        string resendDelayKey = LoginUserKeyConstants.GetResendDelayKey(identifier);

        // Remove OTP and session
        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);
        await _cacheService.RemoveAsync(sessionKey, cancellationToken);
        await _rateLimiterService.RemoveAsync(resendDelayKey, cancellationToken);
    }

    #endregion
}
