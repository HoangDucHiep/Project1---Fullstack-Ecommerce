using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Application.Abstracts.Sms;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Handler for initiating user login with OTP verification.
/// Manages the complete flow of user login initiation including
/// credential validation, rate limiting, OTP generation, session creation, and SMS sending.
/// </summary>
internal sealed class LoginWithOtpCommandHandler : ICommandHandler<LoginWithOtpCommand>
{
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly ICacheService _cacheService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginWithOtpCommandHandler(
        IOtpService otpService,
        ISmsService smsService,
        IRateLimiterService rateLimiterService,
        ICacheService cacheService,
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _otpService = otpService;
        _smsService = smsService;
        _rateLimiterService = rateLimiterService;
        _cacheService = cacheService;
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(LoginWithOtpCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify credentials and get user information
        Result<(string identityUserId, User domainUser)> credentialResult = await ValidateCredentialsAsync(request.Identifier, request.Password, cancellationToken);
        if (credentialResult.IsFailure)
        {
            return credentialResult;
        }

        (string identityUserId, User domainUser) = credentialResult.Value;

        // 2. Check resend delay
        Result resendDelayResult = await CheckResendDelayAsync(request.Identifier, cancellationToken);
        if (resendDelayResult.IsFailure)
        {
            return resendDelayResult;
        }

        // 3. Apply rate limiting
        Result rateLimitResult = await ApplyRateLimitingAsync(request.Identifier, cancellationToken);
        if (rateLimitResult.IsFailure)
        {
            return rateLimitResult;
        }

        // 4. Handle OTP creation
        Result<string> otpResult = await HandleOtpCreationAsync(request.Identifier, cancellationToken);
        if (otpResult.IsFailure)
        {
            return otpResult;
        }

        // 5. Create login session
        Result sessionResult = await CreateLoginSessionAsync(request.Identifier, identityUserId, domainUser.Phone, domainUser.Email, cancellationToken);
        if (sessionResult.IsFailure)
        {
            // Cleanup OTP if session creation fails
            await CleanupOtpAsync(request.Identifier, cancellationToken);
            return sessionResult;
        }

        // 6. Send OTP via SMS (only if phone number exists)
        if (!string.IsNullOrEmpty(domainUser.Phone))
        {
            Result smsResult = await SendOtpSmsAsync(domainUser.Phone, otpResult.Value, cancellationToken);
            if (smsResult.IsFailure)
            {
                // Cleanup on SMS failure
                await CleanupLoginDataAsync(request.Identifier, cancellationToken);
                return smsResult;
            }
        }

        // 7. Apply post-send restrictions
        await ApplyPostSendRestrictionsAsync(request.Identifier, cancellationToken);

        return Result.Success();
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates user credentials and returns user information.
    /// </summary>
    private async Task<Result<(string identityUserId, User domainUser)>> ValidateCredentialsAsync(string identifier, string password, CancellationToken cancellationToken)
    {
        // Verify identity and password
        Result<string> identityResult = await _authenticationService.VerifyIdentityAndPasswordAsync(identifier, password);
        if (identityResult.IsFailure)
        {
            return Result.Failure<(string, User)>(identityResult.Error);
        }

        string identityUserId = identityResult.Value;

        // Get domain user
        User? domainUser = await _userRepository.GetByIdentityIdAsync(identityUserId, cancellationToken);
        if (domainUser == null)
        {
            return Result.Failure<(string, User)>(UserErrors.InvalidCredentials);
        }

        return Result.Success((identityUserId, domainUser));
    }

    /// <summary>
    /// Applies rate limiting to prevent abuse of OTP generation.
    /// </summary>
    private async Task<Result> ApplyRateLimitingAsync(string identifier, CancellationToken cancellationToken)
    {
        string rateLimitKey = LoginUserKeyConstants.GetRateLimiterKey(identifier);

        Result rateLimitResult = await _rateLimiterService.CheckRateLimitAsync(
            rateLimitKey,
            LoginUserKeyConstants.RATE_LIMIT_MAX_ATTEMPTS,
            LoginUserKeyConstants.RATE_LIMIT_WINDOW_SECONDS,
            cancellationToken);

        if (rateLimitResult.IsFailure)
        {
            return rateLimitResult;
        }

        // Increment rate limit counter
        await _rateLimiterService.IncrementAsync(
            rateLimitKey,
            LoginUserKeyConstants.RATE_LIMIT_WINDOW_SECONDS,
            cancellationToken);

        return Result.Success();
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
    /// Handles OTP creation for login.
    /// </summary>
    private async Task<Result<string>> HandleOtpCreationAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);
        var otpTtl = TimeSpan.FromMinutes(LoginUserKeyConstants.OTP_LIFE_TIME_IN_MINUTE);

        // Create new OTP
        return await _otpService.CreateOtpAsync(
            otpKey,
            LoginUserKeyConstants.OTP_LENGTH,
            otpTtl,
            cancellationToken);
    }

    /// <summary>
    /// Creates the login session with user data.
    /// </summary>
    private async Task<Result> CreateLoginSessionAsync(string identifier, string identityUserId, string? phoneNumber, string? email, CancellationToken cancellationToken)
    {
        LoginSession loginSession = new(
            Identifier: identifier,
            IdentityUserId: identityUserId,
            PhoneNumber: phoneNumber,
            Email: email,
            CreatedAt: _dateTimeProvider.UtcNow
        );

        string sessionKey = LoginUserKeyConstants.GetSessionKey(identifier);
        var sessionTtl = TimeSpan.FromMinutes(LoginUserKeyConstants.LOGIN_SESSION_LIFE_TIME_IN_MINUTE);

        await _cacheService.SetAsync(sessionKey, loginSession, sessionTtl, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Sends the OTP to the user via SMS.
    /// </summary>
    private async Task<Result> SendOtpSmsAsync(string phoneNumber, string otp, CancellationToken cancellationToken)
    {
        return await _smsService.SendOtpAsync(phoneNumber, otp, cancellationToken);
    }

    /// <summary>
    /// Applies post-send restrictions (resend delay).
    /// </summary>
    private async Task ApplyPostSendRestrictionsAsync(string identifier, CancellationToken cancellationToken)
    {
        string resendDelayKey = LoginUserKeyConstants.GetResendDelayKey(identifier);

        await _rateLimiterService.LockAsync(
            resendDelayKey,
            LoginUserKeyConstants.OTP_SEND_DELAY_IN_SECOND,
            cancellationToken);
    }

    /// <summary>
    /// Cleans up OTP data in case of failure.
    /// </summary>
    private async Task CleanupOtpAsync(string identifier, CancellationToken cancellationToken)
    {
        string otpKey = LoginUserKeyConstants.GetOtpKey(identifier);
        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);

        string resendDelayKey = LoginUserKeyConstants.GetResendDelayKey(identifier);
        await _rateLimiterService.RemoveAsync(resendDelayKey, cancellationToken);
    }

    /// <summary>
    /// Cleans up all login-related data in case of failure.
    /// </summary>
    private async Task CleanupLoginDataAsync(string identifier, CancellationToken cancellationToken)
    {
        await CleanupOtpAsync(identifier, cancellationToken);

        string sessionKey = LoginUserKeyConstants.GetSessionKey(identifier);
        await _cacheService.RemoveAsync(sessionKey, cancellationToken);
    }

    #endregion
}
