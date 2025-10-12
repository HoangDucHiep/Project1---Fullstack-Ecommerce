using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Abstracts.Otp;
using ECommerceBackend.Application.Abstracts.RateLimiter;
using ECommerceBackend.Application.Abstracts.Sms;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;
internal sealed class LoginWithOtpCommandHandler : ICommandHandler<LoginWithOtpCommand>
{

    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IAuthenticationService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IRateLimiterService _rateLimiterService;

    public LoginWithOtpCommandHandler(
        IOtpService otpService,
        ISmsService smsService,
        IAuthenticationService authService,
        IUserRepository userRepository,
        IRateLimiterService rateLimiterService)
    {
        _otpService = otpService;
        _smsService = smsService;
        _authService = authService;
        _userRepository = userRepository;
        _rateLimiterService = rateLimiterService;
    }



    public async Task<Result> Handle(LoginWithOtpCommand request, CancellationToken cancellationToken)
    {
        // Check if 
        Result<string> checkIdentifier = await _authService.VerifyIdentityAndPasswordAsync(request.Identifier, request.Password);

        if (checkIdentifier.IsFailure)
        {
            return Result.Failure(checkIdentifier.Error);
        }

        string identityUserId = checkIdentifier.Value;

        User existUser = await _userRepository.GetByIdentityIdAsync(identityUserId, cancellationToken);

        if (existUser == null)
        {
            return Result.Failure(checkIdentifier.Error);
        }

        // Check rate limit
        const string RATE_LIMIT_OTP_LOGIN_KEY = "ratelimit:user:login:";

        string rateLimitKey = $"{RATE_LIMIT_OTP_LOGIN_KEY}{existUser.Phone}";


        return null;



    }
}
