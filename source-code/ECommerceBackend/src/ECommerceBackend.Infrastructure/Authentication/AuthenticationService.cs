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


    /// <summary>
    /// Register a new user using their phone number and an optional password.
    /// Needs to verify OTP before calling this method.
    /// Use OtpService to handle OTP generation and verification.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<Result<AuthenticationResult>> InternalRegisterUserWithPhoneAsync(string phoneNumber, string? password = null, bool phoneNumberConfirmed = true)
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
            PhoneNumberConfirmed = phoneNumberConfirmed,
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
    /// Wrapper method for backward compatibility
    /// </summary>
    public async Task<Result<AuthenticationResult>> RegisterUserAsync(string phoneNumber, string? password = null)
    {
        return await InternalRegisterUserWithPhoneAsync(phoneNumber, password);
    }

    /// <summary>
    /// Wrapper method for backward compatibility
    /// </summary>
    public async Task<Result<AuthenticationResult>> LoginAsync(string identifier, string password)
    {
        return await InternalLoginAsync(identifier, password);
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

    /// <summary>
    /// Internal login by identity user ID for OTP-based authentication.
    /// Used when password has already been verified during OTP initiation.
    /// </summary>
    /// <param name="identityUserId">The identity user ID</param>
    /// <returns></returns>
    public async Task<Result<AuthenticationResult>> InternalLoginByIdentityIdAsync(string identityUserId)
    {
        ApplicationIdentityUser? identityUser = await _userManager.FindByIdAsync(identityUserId);

        if (identityUser == null)
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

    public async Task<Result<AuthenticationResult>> InternalLoginAsync(string identifier, string password)
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



    public async Task<Result> LogoutAsync(string userId)
    {
        // Revoke all refresh tokens for the user
        await _refreshTokenRepository.RevokeAllTokensForUserAsync(userId);
        await _identityUnitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken)
    {
        // 1. Find the refresh token in the database
        RefreshToken? storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        
        if (storedRefreshToken == null)
        {
            return Result.Failure<AuthenticationResult>(AuthenticationErrors.InvalidRefreshToken);
        }

        // 2. Check if the refresh token is expired
        if (storedRefreshToken.ExpiresAtUtc <= _dateTimeProvider.UtcNow)
        {
            return Result.Failure<AuthenticationResult>(AuthenticationErrors.RefreshTokenExpired);
        }

        // 3. Check if the refresh token is revoked
        if (storedRefreshToken.IsRevoked)
        {
            // Additional security: If someone tries to use a revoked token, 
            // it might indicate a security breach, so revoke all tokens for this user
            await _refreshTokenRepository.RevokeAllTokensForUserAsync(storedRefreshToken.IdentityUserId);
            await _identityUnitOfWork.SaveChangesAsync();
            
            return Result.Failure<AuthenticationResult>(AuthenticationErrors.RefreshTokenRevoked);
        }

        // 4. Check if the refresh token is already used (Token Reuse Detection)
        if (storedRefreshToken.IsUsed)
        {
            // Security violation: Token reuse detected
            // Revoke all refresh tokens for this user as a security measure
            await _refreshTokenRepository.RevokeAllTokensForUserAsync(storedRefreshToken.IdentityUserId);
            await _identityUnitOfWork.SaveChangesAsync();
            
            return Result.Failure<AuthenticationResult>(AuthenticationErrors.TokenReuseDetected);
        }

        // 5. Get the user associated with the refresh token
        ApplicationIdentityUser? identityUser = await _userManager.FindByIdAsync(storedRefreshToken.IdentityUserId);
        
        if (identityUser == null)
        {
            return Result.Failure<AuthenticationResult>(AuthenticationErrors.UserNotFound);
        }

        // 6. Mark the old refresh token as used
        storedRefreshToken.IsUsed = true;
        storedRefreshToken.ReplacedByToken = _jwtService.GenerateRefreshToken();

        // 7. Generate new tokens
        string newAccessToken = _jwtService.GenerateAccessToken(
            identityUser.Id,
            email: identityUser.Email,
            phoneNumber: identityUser.PhoneNumber);

        string newRefreshToken = _jwtService.GenerateRefreshToken();

        // 8. Create and save new refresh token
        var newRefreshTokenEntity = RefreshToken.Create(
            token: newRefreshToken,
            jwtId: ExtractJwtIdFromToken(newAccessToken),
            expiresAtUtc: _dateTimeProvider.UtcNow.AddDays(_jwtService.GetRefreshTokenExpirationInDays()),
            identityUserId: identityUser.Id
        );

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
        await _identityUnitOfWork.SaveChangesAsync();

        return Result.Success(new AuthenticationResult(
            AccessToken: newAccessToken,
            RefreshToken: newRefreshToken,
            AccessTokenExpiration: _dateTimeProvider.UtcNow.AddMinutes(_jwtService.GetAccessTokenExpirationInMinutes()).UtcDateTime,
            RefreshTokenExpiration: newRefreshTokenEntity.ExpiresAtUtc.UtcDateTime,
            IdentityUserId: identityUser.Id.ToString()));
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
