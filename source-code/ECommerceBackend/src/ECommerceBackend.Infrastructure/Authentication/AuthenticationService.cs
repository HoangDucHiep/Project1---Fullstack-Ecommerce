using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;
using ECommerceBackend.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthenticationResult>> RegisterUserAsync(string phoneNumber, string? email = null, string? password = null)
    {
        // Check if phone number already exists
        ApplicationUser? existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        if (existingUser != null)
        {
            return Result.Failure<AuthenticationResult>(new Error("PhoneExists", "Phone number already registered", ErrorType.Conflict));
        }

        // Check if email already exists (if provided)
        if (!string.IsNullOrEmpty(email))
        {
            ApplicationUser? existingEmailUser = await _userManager.FindByEmailAsync(email);
            if (existingEmailUser != null)
            {
                return Result.Failure<AuthenticationResult>(new Error("EmailExists", "Email already registered", ErrorType.Conflict));
            }
        }

        // Create Identity User
        var identityUser = new ApplicationUser
        {
            UserName = phoneNumber, // Use phone as username for regular users
            PhoneNumber = phoneNumber,
            Email = email,
            EmailConfirmed = false,
            IsPhoneNumberVerified = false // Will be verified via OTP later
        };

        IdentityResult result = !string.IsNullOrEmpty(password)
            ? await _userManager.CreateAsync(identityUser, password)
            : await _userManager.CreateAsync(identityUser);

        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AuthenticationResult>(new Error("RegistrationFailed", errors, ErrorType.Validation));
        }

        // Create Domain User
        var domainUser = User.Create(identityUser.Id, email ?? "", phoneNumber);
        _userRepository.Add(domainUser);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        string accessToken = _jwtService.GenerateAccessToken(identityUser.Id, email ?? "", phoneNumber);
        string refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days
        await _userManager.UpdateAsync(identityUser);

        return Result.Success(new AuthenticationResult(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15), // Access token expiry
            identityUser.Id
        ));
    }

    public async Task<Result<AuthenticationResult>> RegisterEmployeeAsync(string email, string password)
    {
        // Check if email already exists
        ApplicationUser? existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Result.Failure<AuthenticationResult>(new Error("EmailExists", "Email already registered", ErrorType.Conflict));
        }

        // Create Identity User for employee
        var identityUser = new ApplicationUser
        {
            UserName = email, // Use email as username for employees
            Email = email,
            EmailConfirmed = true, // Employees don't need email verification
            IsPhoneNumberVerified = true // Employees don't need phone verification
        };

        IdentityResult result = await _userManager.CreateAsync(identityUser, password);
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AuthenticationResult>(new Error("RegistrationFailed", errors, ErrorType.Validation));
        }

        // Create Domain User
        var domainUser = User.Create(identityUser.Id, email, ""); // No phone for employees initially
        _userRepository.Add(domainUser);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        string accessToken = _jwtService.GenerateAccessToken(identityUser.Id, email);
        string refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(identityUser);

        return Result.Success(new AuthenticationResult(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15),
            identityUser.Id
        ));
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(string identifier, string password)
    {
        // Try to find user by email or phone
        ApplicationUser? user = await _userManager.FindByEmailAsync(identifier) ??
                   await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier);

        if (user == null || !user.IsActive)
        {
            return Result.Failure<AuthenticationResult>(new Error("InvalidCredentials", "Invalid credentials", ErrorType.Validation));
        }

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Result.Failure<AuthenticationResult>(new Error("InvalidCredentials", "Invalid credentials", ErrorType.Validation));
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Generate tokens
        string accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email ?? "", user.PhoneNumber);
        string refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthenticationResult(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15),
            user.Id
        ));
    }

    public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken)
    {
        ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow || !user.IsActive)
        {
            return Result.Failure<AuthenticationResult>(new Error("InvalidRefreshToken", "Invalid or expired refresh token", ErrorType.Validation));
        }

        // Generate new tokens
        string accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email ?? "", user.PhoneNumber);
        string newRefreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthenticationResult(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(15),
            user.Id
        ));
    }

    public async Task<Result> LogoutAsync(string userId)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("UserNotFound", "User not found", ErrorType.NotFound));
        }

        // Clear refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("UserNotFound", "User not found", ErrorType.NotFound));
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("PasswordChangeFailed", errors, ErrorType.Validation));
        }

        return Result.Success();
    }
}
