using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;
using ECommerceBackend.Infrastructure.Identity;
using ECommerceBackend.Infrastructure.IdentityAuthen;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IAuthenticationService = ECommerceBackend.Application.Abstracts.Authentication.IAuthenticationService;

namespace ECommerceBackend.Infrastructure.Authentication;
public class AuthenticationService : IAuthenticationService
{

    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly SignInManager<ApplicationIdentityUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthenticationService(
        UserManager<ApplicationIdentityUser> userManager,
        SignInManager<ApplicationIdentityUser> signInManager,
        IJwtService jwtService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result<AuthenticationResult>> RegisterUserAsync(string phoneNumber, string? password = null)
    {
        // Check if phone number is already registered
        ApplicationIdentityUser? existing = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        if (existing != null)
        {
            return Result.Failure<AuthenticationResult>(UserErrors.PhoneNumberAlreadyRegistered(phoneNumber));
        }

        // Create new identity user
        var identityUser = new ApplicationIdentityUser
        {
            // random string as user name
            UserName = Guid.NewGuid().ToString("N")[..10],
            PhoneNumber = phoneNumber,
            PhoneNumberConfirmed = true,
        };

        IdentityResult result = !string.IsNullOrEmpty(password)
            ? await _userManager.CreateAsync(identityUser, password)
            : await _userManager.CreateAsync(identityUser);

        if (!result.Succeeded)
        {
            string errors = string.Join(",", result.Errors.Select(e => e.Description));
            return Result.Failure<AuthenticationResult>(Error.Validation("Authentication.RegisterUserFailed", errors));
        }

        // Create domain user
        var domainUser = User.Create(identityUser.UserName, identityUser.Id, phone: phoneNumber);
        _userRepository.Add(domainUser);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        string accessToken = _jwtService.GenerateAccessToken(identityUser.Id, phoneNumber: phoneNumber);
        string refreshToken = _jwtService.GenerateRefreshToken();

        var rt = RefreshToken.Create(
            token: refreshToken,
            jwtId: ExtractJwtIdFromToken(accessToken),
            expiresAtUtc: _dateTimeProvider.UtcNow.AddDays(_jwtService.GetRefreshTokenExpirationInDays()),
            identityUserId: identityUser.Id
            );

        await _refreshTokenRepository.AddAsync(rt);

        await _identityUnitOfWork.SaveChangesAsync();

        return Result.Success(new AuthenticationResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiration: _dateTimeProvider.UtcNow.AddMinutes(_jwtService.GetAccessTokenExpirationInMinutes()).UtcDateTime,
            RefreshTokenExpiration: rt.ExpiresAtUtc.UtcDateTime,
            IdentityUserId: identityUser.Id.ToString()));
    }

    public Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<Result<string>> VerifyIdentityAndPasswordAsync(string identifier, string password)
    {
        // Determine if identifier is email or phone number
        bool isEmail = identifier.Contains('@');

        ApplicationIdentityUser? identityUser = !isEmail
            ? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier)
            : await _userManager.FindByEmailAsync(identifier);

        if (identityUser == null)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }


        // Check password
        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, password, lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        return identityUser.Id;
    }


    public async Task<Result<AuthenticationResult>> LoginAsync(string identifier, string password)
    {
        // Determine if identifier is email or phone number
        bool isEmail = identifier.Contains('@');

        ApplicationIdentityUser? identityUser = !isEmail
            ? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier)
            : await _userManager.FindByEmailAsync(identifier);

        if (identityUser == null)
        {
            return Result.Failure<AuthenticationResult>(UserErrors.InvalidCredentials);
        }

        // Check password
        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, password, lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            return Result.Failure<AuthenticationResult>(UserErrors.InvalidCredentials);
        }

        // Generate tokens
        string accessToken = _jwtService.GenerateAccessToken(
            identityUser.Id,
            email: identityUser.Email,
            phoneNumber: identityUser.PhoneNumber);

        string refreshToken = _jwtService.GenerateRefreshToken();

        // Create and save refresh token
        var rt = RefreshToken.Create(
            token: refreshToken,
            jwtId: ExtractJwtIdFromToken(accessToken),
            expiresAtUtc: _dateTimeProvider.UtcNow.AddDays(_jwtService.GetRefreshTokenExpirationInDays()),
            identityUserId: identityUser.Id
        );

        await _refreshTokenRepository.AddAsync(rt);
        await _identityUnitOfWork.SaveChangesAsync();

        return Result.Success(new AuthenticationResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiration: _dateTimeProvider.UtcNow.AddMinutes(_jwtService.GetAccessTokenExpirationInMinutes()).UtcDateTime,
            RefreshTokenExpiration: rt.ExpiresAtUtc.UtcDateTime,
            IdentityUserId: identityUser.Id.ToString()));
    }



    public Task<Result> LogoutAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthenticationResult>> RegisterEmployeeAsync(string email, string password)
    {
        throw new NotImplementedException();
    }


    private static string ExtractJwtIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        Claim? jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

        return jtiClaim?.Value ?? Guid.NewGuid().ToString();
    }


}
