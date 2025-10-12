using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Application.Abstracts.Encryption;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Application.Abstracts.Sms;
using ECommerceBackend.Application.Authentication.Register;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Authentication.RegisterUserWithOtp;

/// <summary>
/// Handler for initiating user registration with OTP verification.
/// Manages the complete flow of user registration initiation including
/// rate limiting, OTP generation, session creation, and SMS sending.
/// </summary>
internal sealed class RegisterWithOtpCommandHandler : ICommandHandler<RegisterWithOtpCommand>
{
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly IEncryptionService _encryptionService;
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterWithOtpCommandHandler(
        IOtpService otpService,
        ISmsService smsService,
        IRateLimiterService rateLimiterService,
        IEncryptionService encryptionService,
        ICacheService cacheService,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _otpService = otpService;
        _smsService = smsService;
        _rateLimiterService = rateLimiterService;
        _encryptionService = encryptionService;
        _cacheService = cacheService;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(RegisterWithOtpCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate phone number uniqueness
        Result phoneValidationResult = await ValidatePhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (phoneValidationResult.IsFailure)
        {
            return phoneValidationResult;
        }


        // 3. Check resend delay
        Result resendDelayResult = await CheckResendDelayAsync(request.PhoneNumber, cancellationToken);
        if (resendDelayResult.IsFailure)
        {
            return resendDelayResult;
        }


        // 2. Apply rate limiting
        Result rateLimitResult = await ApplyRateLimitingAsync(request.PhoneNumber, cancellationToken);
        if (rateLimitResult.IsFailure)
        {
            return rateLimitResult;
        }

        // 4. Handle OTP creation or regeneration
        Result<string> otpResult = await HandleOtpCreationAsync(request.PhoneNumber, cancellationToken);
        if (otpResult.IsFailure)
        {
            return otpResult;
        }

        // 5. Create or update registration session
        Result sessionResult = await CreateRegistrationSessionAsync(request, cancellationToken);
        if (sessionResult.IsFailure)
        {
            // Cleanup OTP if session creation fails
            await CleanupOtpAsync(request.PhoneNumber, cancellationToken);
            return sessionResult;
        }

        // 6. Send OTP via SMS
        Result smsResult = await SendOtpSmsAsync(request.PhoneNumber, otpResult.Value, cancellationToken);
        if (smsResult.IsFailure)
        {
            // Cleanup on SMS failure
            await CleanupRegistrationDataAsync(request.PhoneNumber, cancellationToken);
            return smsResult;
        }

        // 7. Apply post-send restrictions
        await ApplyPostSendRestrictionsAsync(request.PhoneNumber, cancellationToken);

        return Result.Success();
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates that the phone number is not already registered.
    /// </summary>
    private async Task<Result> ValidatePhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        User? existingUser = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Failure(UserErrors.PhoneAlreadyExists(phoneNumber));
        }

        return Result.Success();
    }

    /// <summary>
    /// Applies rate limiting to prevent abuse of OTP generation.
    /// </summary>
    private async Task<Result> ApplyRateLimitingAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string rateLimitKey = RegisterUserKeyConstants.GetRateLimiterKey(phoneNumber);

        Result rateLimitResult = await _rateLimiterService.CheckRateLimitAsync(
            rateLimitKey,
            RegisterUserKeyConstants.RATE_LIMIT_MAX_ATTEMPTS,
            RegisterUserKeyConstants.RATE_LIMIT_WINDOW_SECONDS,
            cancellationToken);

        if (rateLimitResult.IsFailure)
        {
            return rateLimitResult;
        }

        // Increment rate limit counter
        await _rateLimiterService.IncrementAsync(
            rateLimitKey,
            RegisterUserKeyConstants.RATE_LIMIT_WINDOW_SECONDS,
            cancellationToken);

        return Result.Success();
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
    /// Handles OTP creation or regeneration based on existing state.
    /// </summary>
    private async Task<Result<string>> HandleOtpCreationAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);
        var otpTtl = TimeSpan.FromMinutes(RegisterUserKeyConstants.OTP_LIFE_TIME_IN_MINUTE);


        // Create new OTP
        return await _otpService.CreateOtpAsync(
            otpKey,
            RegisterUserKeyConstants.OTP_LENGTH,
            otpTtl,
            cancellationToken);
    }

    /// <summary>
    /// Creates or updates the registration session with user data.
    /// </summary>
    private async Task<Result> CreateRegistrationSessionAsync(RegisterWithOtpCommand request, CancellationToken cancellationToken)
    {
        Result<string> encryptedPassword = _encryptionService.Encrypt(request.Password);
        if (encryptedPassword.IsFailure)
        {
            return encryptedPassword;
        }

        var registrationSession = new RegistrationSession(
            PhoneNumber: request.PhoneNumber,
            PasswordHash: encryptedPassword.Value,
            CreatedAt: _dateTimeProvider.UtcNow
        );

        string sessionKey = RegisterUserKeyConstants.GetSessionKey(request.PhoneNumber);
        var sessionTtl = TimeSpan.FromMinutes(RegisterUserKeyConstants.REGISTRATION_SESSION_LIFE_TIME_IN_MINUTE);

        await _cacheService.SetAsync(sessionKey, registrationSession, sessionTtl, cancellationToken);

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
    private async Task ApplyPostSendRestrictionsAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string resendDelayKey = RegisterUserKeyConstants.GetResendDelayKey(phoneNumber);

        await _rateLimiterService.LockAsync(
            resendDelayKey,
            RegisterUserKeyConstants.OTP_SEND_DELAY_IN_SECOND,
            cancellationToken);
    }

    /// <summary>
    /// Cleans up OTP data in case of failure.
    /// </summary>
    private async Task CleanupOtpAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);
        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);

        string resendDelayKey = RegisterUserKeyConstants.GetResendDelayKey(phoneNumber);
        await _rateLimiterService.RemoveAsync(resendDelayKey, cancellationToken);
    }

    /// <summary>
    /// Cleans up all registration-related data in case of failure.
    /// </summary>
    private async Task CleanupRegistrationDataAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        await CleanupOtpAsync(phoneNumber, cancellationToken);

        string sessionKey = RegisterUserKeyConstants.GetSessionKey(phoneNumber);
        await _cacheService.RemoveAsync(sessionKey, cancellationToken);
    }

    #endregion
}
