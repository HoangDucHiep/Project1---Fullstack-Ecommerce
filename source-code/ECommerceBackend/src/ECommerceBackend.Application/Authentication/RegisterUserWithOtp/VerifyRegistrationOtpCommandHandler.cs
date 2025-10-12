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
        ICacheService redisSeervice,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _authenticationService = authenticationService;
        _userContext = userContext;
        _otpService = otpService;
        _rateLimiterService = rateLimiterService;
        _encryptionService = encryptionService;
        _redisService = redisSeervice;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task<Result<AuthenticationResult>> Handle(VerifyRegistrationOtpCommand request, CancellationToken cancellationToken)
    {
        string otpKey = $"otp:register:{request.PhoneNumber}";
        string sessionKey = $"session:register:{request.PhoneNumber}";
        string lockKey = $"lock:register:{request.PhoneNumber}";

        // Check if rate limit locked
        Result<bool> lockedResult = await _rateLimiterService.IsLockedAsync(lockKey, cancellationToken);

        if (lockedResult.IsSuccess && lockedResult.Value)
        {
            return Result.Failure<AuthenticationResult>(RateLimiterErrors.Locked(15));
        }

        // Verify OTP
        Result otpVerifyResult = await _otpService.VerifyOtpAsync(otpKey, request.Otp, 5, cancellationToken);

        if (otpVerifyResult.IsFailure)
        {
            // Check if max attempts reached to apply lock
            if (otpVerifyResult.Error.Code == OtpErrors.MaxAttemptsExceeded.Code)
            {
                await _rateLimiterService.LockAsync(lockKey, durationSeconds: 900, cancellationToken);
                await _redisService.RemoveAsync(otpKey, cancellationToken);
            }

            return Result.Failure<AuthenticationResult>(otpVerifyResult.Error);
        }

        // Get registration session data
        RegistrationSession sessionResult = await _redisService.GetAsync<RegistrationSession>(sessionKey, cancellationToken);

        if (sessionResult is null)
        {
            return Result.Failure<AuthenticationResult>(OtpErrors.SessionExpired);
        }

        // Decrypt password
        Result<string> decryptedPassword = _encryptionService.Decrypt(sessionResult.PasswordHash);
        if (decryptedPassword.IsFailure)
        {
            return Result.Failure<AuthenticationResult>(decryptedPassword.Error);
        }

        // Register user
        Result<AuthenticationResult> registrationResult = await _authenticationService.RegisterUserAsync(
             phoneNumber: sessionResult.PhoneNumber,
             password: decryptedPassword.Value
         );

        if (registrationResult.IsFailure)
        {
            return Result.Failure<AuthenticationResult>(registrationResult.Error);
        }

        // Clean up
        await _otpService.RemoveOtpAsync(otpKey, cancellationToken);
        await _redisService.RemoveAsync(sessionKey, cancellationToken);

        return Result.Success(registrationResult.Value);
    }
}
