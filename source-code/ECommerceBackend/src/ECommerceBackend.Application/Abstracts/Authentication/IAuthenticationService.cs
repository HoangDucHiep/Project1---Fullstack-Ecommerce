using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Authentication;

public record AuthenticationResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration,
    string IdentityUserId);

/// HDHiep - 10/07/2025
/// <summary>
/// Authentication service interface for user registration, login, token refresh, and logout
/// </summary>
/// remarks>
/// - RegisterUserAsync: Register a new user with phone number and optional password
/// - RegisterEmployeeAsync: Register a new employee with email and password
/// - LoginAsync: Login with identifier (phone number or email) and password
/// - RefreshTokenAsync: Refresh access token using a valid refresh token
/// - LogoutAsync: Invalidate refresh tokens for a user
/// - ChangePasswordAsync: Change user's password
/// </remarks>
public interface IAuthenticationService
{
    Task<Result<AuthenticationResult>> RegisterUserAsync(string phoneNumber, string? password = null);
    Task<Result<AuthenticationResult>> RegisterEmployeeAsync(string email, string password);

    Task<Result<string>> VerifyIdentityAndPasswordAsync(string identifier, string password);
    Task<Result<AuthenticationResult>> LoginAsync(string identifier, string password);

    Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken);
    Task<Result> LogoutAsync(string userId);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
