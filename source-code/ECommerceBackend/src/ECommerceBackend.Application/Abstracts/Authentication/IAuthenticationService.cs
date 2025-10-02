using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Authentication;
public record AuthenticationResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId
);

public interface IAuthenticationService
{
    Task<Result<AuthenticationResult>> RegisterUserAsync(string phoneNumber, string? email = null, string? password = null);
    Task<Result<AuthenticationResult>> RegisterEmployeeAsync(string email, string password);
    Task<Result<AuthenticationResult>> LoginAsync(string identifier, string password);
    Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken);
    Task<Result> LogoutAsync(string userId);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
