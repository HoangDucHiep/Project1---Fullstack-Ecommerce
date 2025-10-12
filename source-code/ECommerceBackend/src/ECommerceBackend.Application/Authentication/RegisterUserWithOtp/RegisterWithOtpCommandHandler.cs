using ECommerceBackend.Application.Abstracts.Authentication;
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
internal sealed class RegisterWithOtpCommandHandler : ICommandHandler<RegisterWithOtpCommand>
{
    private readonly IUserContext _userContext;
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly IEncryptionService _encryptionService;
    private readonly ICacheService _redisService;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterWithOtpCommandHandler(
        IUserContext userContext,
        IOtpService otpService,
        ISmsService smsService,
        IRateLimiterService rateLimiterService,
        IEncryptionService encryptionService,
        ICacheService redisSeervice,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userContext = userContext;
        _otpService = otpService;
        _smsService = smsService;
        _rateLimiterService = rateLimiterService;
        _encryptionService = encryptionService;
        _redisService = redisSeervice;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(RegisterWithOtpCommand request, CancellationToken cancellationToken)
    {
        // Check  if phone number already exists
        User existUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

        if (existUser is not null)
        {
            return Result.Failure(UserErrors.PhoneAlreadyExists(request.PhoneNumber));
        }

        // Rate limit check - Send OTP max 3 times per 15 minutes
        // TODO: Declare some constants for rate limit values
        string rateLimitKey = $"ratelimit:register:{request.PhoneNumber}";
        Result rateLimitResult = await _rateLimiterService.CheckRateLimitAsync(rateLimitKey, maxAttempts: 3, windowSeconds: 900, cancellationToken);

        if (rateLimitResult.IsFailure)
        {
            return rateLimitResult;
        }

        // Generate OTP
        string otp = _otpService.GenerateOtp();
        string otpKey = $"otp:register:{request.PhoneNumber}";

        Result otpStoreResult = await _otpService.SetOtpAsync(otpKey, otp, TimeSpan.FromMinutes(5), cancellationToken);

        if (otpStoreResult.IsFailure)
        {
            return otpStoreResult;
        }

        // Generate registration session data for temp storage
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

        string sessionKey = $"session:register:{request.PhoneNumber}";
        await _redisService.SetAsync(sessionKey, registrationSession, TimeSpan.FromMinutes(30), cancellationToken);

        // Send OTP via SMS
        Result smsResult = await _smsService.SendOtpAsync(request.PhoneNumber, otp, cancellationToken);

        if (smsResult.IsFailure)
        {
            // Clean up stuffs
            await _otpService.RemoveOtpAsync(otpKey, cancellationToken);
            await _redisService.RemoveAsync(sessionKey, cancellationToken);
            return smsResult;
        }

        // 6. Increment rate limit counter
        await _rateLimiterService.IncrementAsync(rateLimitKey, 900, cancellationToken);

        return Result.Success();
    }
}
