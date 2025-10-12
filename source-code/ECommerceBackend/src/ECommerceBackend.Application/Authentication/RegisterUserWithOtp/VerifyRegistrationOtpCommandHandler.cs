using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Application.Abstracts.Encryption;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Application.Authentication.Register;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Authentication.RegisterUserWithOtp;

/// <summary>
/// Handler for verifying OTP during user registration.
/// Completes the registration process by verifying the OTP,
/// creating the user account, and returning authentication tokens.
/// </summary>
public class VerifyRegistrationOtpCommandHandler : ICommandHandler<VerifyRegistrationOtpCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserContext _userContext;
    private readonly IOtpService _otpService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly IEncryptionService _encryptionService;
    private readonly ICacheService _redisService;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public VerifyRegistrationOtpCommandHandler(
        IAuthenticationService authenticationService,
        IUserContext userContext,
        IOtpService otpService,
        IRateLimiterService rateLimiterService,
        IEncryptionService encryptionService,
        ICacheService redisService,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _authenticationService = authenticationService;
        _userContext = userContext;
        _otpService = otpService;
        _rateLimiterService = rateLimiterService;
        _encryptionService = encryptionService;
        _redisService = redisService;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<AuthenticationResult>> Handle(VerifyRegistrationOtpCommand request, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(request.PhoneNumber);
        string sessionKey = RegisterUserKeyConstants.GetSessionKey(request.PhoneNumber);
        string verificationLockKey = $"lock:verification:{request.PhoneNumber}";

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
            RegisterUserKeyConstants.OTP_MAX_VERIFICATION_ATTEMPTS,
            cancellationToken);

        if (otpVerifyResult.IsFailure)
        {
            // Apply verification lock on max attempts exceeded
            if (otpVerifyResult.Error.Code == OtpErrors.MaxAttemptsExceeded.Code)
            {
                await _rateLimiterService.LockAsync(verificationLockKey, 900, cancellationToken); // 15 minutes lock
                await CleanupRegistrationDataAsync(request.PhoneNumber, cancellationToken);
            }

            return Result.Failure<AuthenticationResult>(otpVerifyResult.Error);
        }

        // Get registration session data
        RegistrationSession? sessionResult = await _redisService.GetAsync<RegistrationSession>(sessionKey, cancellationToken);

        if (sessionResult is null)
        {
            return Result.Failure<AuthenticationResult>(OtpErrors.SessionExpired);
        }

        // Decrypt password
        Result<string> decryptedPassword = _encryptionService.Decrypt(sessionResult.PasswordHash);
        if (decryptedPassword.IsFailure)
        {
            await CleanupRegistrationDataAsync(request.PhoneNumber, cancellationToken);
            return Result.Failure<AuthenticationResult>(decryptedPassword.Error);
        }

        // Register user
        Result<AuthenticationResult> registrationResult = await _authenticationService.RegisterUserAsync(
            phoneNumber: sessionResult.PhoneNumber,
            password: decryptedPassword.Value
        );

        if (registrationResult.IsFailure)
        {
            await CleanupRegistrationDataAsync(request.PhoneNumber, cancellationToken);
            return Result.Failure<AuthenticationResult>(registrationResult.Error);
        }

        // Clean up successful registration data
        await CleanupRegistrationDataAsync(request.PhoneNumber, cancellationToken);

        return Result.Success(registrationResult.Value);
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
    /// Cleans up all registration-related data.
    /// </summary>
    private async Task CleanupRegistrationDataAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        string otpKey = RegisterUserKeyConstants.GetOtpKey(phoneNumber);
        string sessionKey = RegisterUserKeyConstants.GetSessionKey(phoneNumber);
        string resendDelayKey = RegisterUserKeyConstants.GetResendDelayKey(phoneNumber);

        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);
        await _redisService.RemoveAsync(sessionKey, cancellationToken);
        await _rateLimiterService.RemoveAsync(resendDelayKey, cancellationToken);
    }

    #endregion
}
