using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Handler for verifying OTP during user login.
/// Completes the login process by verifying the OTP,
/// and returning authentication tokens.
/// </summary>
public class VerifyLoginOtpCommandHandler : ICommandHandler<VerifyLoginOtpCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IOtpService _otpService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly ICacheService _cacheService;

    public VerifyLoginOtpCommandHandler(
        IAuthenticationService authenticationService,
        IOtpService otpService,
        IRateLimiterService rateLimiterService,
        ICacheService cacheService)
    {
        _authenticationService = authenticationService;
        _otpService = otpService;
        _rateLimiterService = rateLimiterService;
        _cacheService = cacheService;
    }

    public async Task<Result<AuthenticationResult>> Handle(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(request.Identifier);
        string sessionKey = LoginUserKeyConstants.GetSessionKey(request.Identifier);
        string verificationLockKey = $"lock:verification:login:{request.Identifier}";

        // Check if verification is locked due to too many failed attempts
        Result<bool> lockedResult = await _rateLimiterService.IsLockedAsync(verificationLockKey, cancellationToken);

        if (lockedResult.IsSuccess && lockedResult.Value)
        {
            int lockTimeLeft = await GetLockTimeLeftAsync(verificationLockKey, cancellationToken);
            return Result.Failure<AuthenticationResult>(RateLimiterErrors.Locked(lockTimeLeft / 60));
        }

        // Verify OTP
        Result otpVerifyResult = await _otpService.VerifyOtpAsync(
            otpKey,
            request.Otp,
            LoginUserKeyConstants.OTP_MAX_VERIFICATION_ATTEMPTS,
            cancellationToken);

        if (otpVerifyResult.IsFailure)
        {
            // Apply verification lock on max attempts exceeded
            if (otpVerifyResult.Error.Code == OtpErrors.MaxAttemptsExceeded.Code)
            {
                await _rateLimiterService.LockAsync(verificationLockKey, 900, cancellationToken); // 15 minutes lock
                await CleanupLoginDataAsync(request.Identifier, cancellationToken);
            }

            return Result.Failure<AuthenticationResult>(otpVerifyResult.Error);
        }

        // Get login session data
        LoginSession? sessionResult = await _cacheService.GetAsync<LoginSession>(sessionKey, cancellationToken);

        if (sessionResult is null)
        {
            return Result.Failure<AuthenticationResult>(OtpErrors.SessionExpired);
        }

        // Generate authentication tokens using the identity user ID
        Result<AuthenticationResult> loginResult = await _authenticationService.InternalLoginByIdentityIdAsync(
            sessionResult.IdentityUserId);

        if (loginResult.IsFailure)
        {
            await CleanupLoginDataAsync(request.Identifier, cancellationToken);
            return Result.Failure<AuthenticationResult>(loginResult.Error);
        }

        // Clean up successful login data
        await CleanupLoginDataAsync(request.Identifier, cancellationToken);

        return Result.Success(loginResult.Value);
    }

    #region Private Helper Methods

    /// <summary>
    /// Gets the remaining lock time in seconds.
    /// </summary>
    private async Task<int> GetLockTimeLeftAsync(string lockKey, CancellationToken cancellationToken)
    {
        Result<int> lockTimeResult = await _rateLimiterService.GetLockSecondsAliveLeft(lockKey, cancellationToken);
        return lockTimeResult.IsSuccess ? lockTimeResult.Value : 0;
    }

    /// <summary>
    /// Cleans up all login-related data.
    /// </summary>
    private async Task CleanupLoginDataAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);
        string sessionKey = LoginUserKeyConstants.GetSessionKey(identifier);
        string resendDelayKey = LoginUserKeyConstants.GetResendDelayKey(identifier);

        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);
        await _cacheService.RemoveAsync(sessionKey, cancellationToken);
        await _rateLimiterService.RemoveAsync(resendDelayKey, cancellationToken);
    }

    #endregion
}
