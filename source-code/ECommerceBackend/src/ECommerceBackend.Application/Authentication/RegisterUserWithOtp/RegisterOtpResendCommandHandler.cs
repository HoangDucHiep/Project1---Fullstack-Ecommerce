using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Application.Abstracts.Sms;
using ECommerceBackend.Application.Authentication.Register;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.RegisterUserWithOtp;

/// <summary>
/// Handler for resending OTP during user registration process.
/// Manages OTP regeneration with proper validation of session state,
/// resend limits, and rate limiting.
/// Returns the number of resend attempts remaining.
/// </summary>
internal sealed class RegisterOtpResendCommandHandler : ICommandHandler<RegisterOtpResendCommand, int>
{
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly ICacheService _cacheService;

    public RegisterOtpResendCommandHandler(
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

    public async Task<Result<int>> Handle(RegisterOtpResendCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate registration session exists
        Result<RegistrationSession> sessionValidationResult = await ValidateRegistrationSessionAsync(request.Phone, cancellationToken);
        if (sessionValidationResult.IsFailure)
        {
            return Result.Failure<int>(sessionValidationResult.Error);
        }

        // 2. Check resend delay
        Result resendDelayResult = await CheckResendDelayAsync(request.Phone, cancellationToken);
        if (resendDelayResult.IsFailure)
        {
            return Result.Failure<int>(resendDelayResult.Error);
        }

        // 3. Validate OTP exists and check resend limits
        Result otpValidationResult = await ValidateOtpAndResendLimitsAsync(request.Phone, cancellationToken);
        if (otpValidationResult.IsFailure)
        {
            return Result.Failure<int>(otpValidationResult.Error);
        }

        // 4. Regenerate OTP
        Result<string> newOtpResult = await RegenerateOtpAsync(request.Phone, cancellationToken);
        if (newOtpResult.IsFailure)
        {
            return Result.Failure<int>(newOtpResult.Error);
        }

        // 5. Update session expiration
        await RefreshRegistrationSessionAsync(request.Phone, sessionValidationResult.Value, cancellationToken);

        // 6. Send OTP via SMS
        Result smsResult = await SendOtpSmsAsync(request.Phone, newOtpResult.Value, cancellationToken);
        if (smsResult.IsFailure)
        {
            // Cleanup on SMS failure
            await CleanupFailedResendAsync(request.Phone, cancellationToken);
            return Result.Failure<int>(smsResult.Error);
        }

        // 7. Apply resend delay
        await ApplyResendDelayAsync(request.Phone, cancellationToken);

        // 8. Return remaining resend attempts
        Result<int> resendLeftResult = await GetRemainingResendAttemptsAsync(request.Phone, cancellationToken);
        return resendLeftResult.IsSuccess
            ? Result.Success(resendLeftResult.Value)
            : Result.Failure<int>(resendLeftResult.Error);
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates that the registration session exists and is valid.
    /// </summary>
    private async Task<Result<RegistrationSession>> ValidateRegistrationSessionAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string sessionKey = RegisterUserKeyConstants.GetSessionKey(phoneNumber);
        RegistrationSession? session = await _cacheService.GetAsync<RegistrationSession>(sessionKey, cancellationToken);

        if (session is null)
        {
            return Result.Failure<RegistrationSession>(OtpErrors.SessionExpired);
        }

        return Result.Success(session);
    }

    /// <summary>
    /// Checks if the user is still within the resend delay period.
    /// </summary>
    private async Task<Result> CheckResendDelayAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string resendDelayKey = RegisterUserKeyConstants.GetResendDelayKey(phoneNumber);

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
    private async Task<Result> ValidateOtpAndResendLimitsAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);

        // Check if OTP exists
        Result<OtpData> otpResult = await _otpService.GetOtpAsync(otpKey, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure(OtpErrors.SessionExpired);
        }

        // Check resend limit
        Result<int> resendLeft = await _otpService.GetResendLeft(
            otpKey,
            RegisterUserKeyConstants.OTP_MAX_RESEND,
            cancellationToken);

        if (resendLeft.IsSuccess && resendLeft.Value <= 0)
        {
            double blockTimeSeconds = TimeSpan.FromMinutes(RegisterUserKeyConstants.RESEND_LIMIT_BLOCK_TIME_IN_MINUTE).TotalSeconds;
            return Result.Failure(OtpErrors.ResendLimitReached((int)blockTimeSeconds));
        }

        return Result.Success();
    }

    /// <summary>
    /// Regenerates the OTP for the user.
    /// </summary>
    private async Task<Result<string>> RegenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);
        var otpTtl = TimeSpan.FromMinutes(RegisterUserKeyConstants.OTP_LIFE_TIME_IN_MINUTE);

        return await _otpService.RegenerateOtpAsync(otpKey, otpTtl, cancellationToken);
    }

    /// <summary>
    /// Refreshes the registration session expiration time.
    /// </summary>
    private async Task RefreshRegistrationSessionAsync(string phoneNumber, RegistrationSession session, CancellationToken cancellationToken)
    {
        string sessionKey = RegisterUserKeyConstants.GetSessionKey(phoneNumber);
        var sessionTtl = TimeSpan.FromMinutes(RegisterUserKeyConstants.REGISTRATION_SESSION_LIFE_TIME_IN_MINUTE);

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
    private async Task ApplyResendDelayAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string resendDelayKey = RegisterUserKeyConstants.GetResendDelayKey(phoneNumber);

        await _rateLimiterService.LockAsync(
            resendDelayKey,
            RegisterUserKeyConstants.OTP_RESEND_DELAY_IN_SECOND,
            cancellationToken);
    }

    /// <summary>
    /// Gets the remaining number of resend attempts.
    /// </summary>
    private async Task<Result<int>> GetRemainingResendAttemptsAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);

        return await _otpService.GetResendLeft(
            otpKey,
            RegisterUserKeyConstants.OTP_MAX_RESEND,
            cancellationToken);
    }

    /// <summary>
    /// Cleans up data when SMS sending fails.
    /// </summary>
    private async Task CleanupFailedResendAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);
        string sessionKey = RegisterUserKeyConstants.GetSessionKey(phoneNumber);
        string resendDelayKey = RegisterUserKeyConstants.GetResendDelayKey(phoneNumber);

        // Remove OTP and session
        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);
        await _cacheService.RemoveAsync(sessionKey, cancellationToken);
        await _rateLimiterService.RemoveAsync(resendDelayKey, cancellationToken);
    }

    #endregion
}
