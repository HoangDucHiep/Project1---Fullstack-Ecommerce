namespace ECommerceBackend.Application.Abstracts.Authentication;


/// HDHiep - 10/07/2025
/// <summary>
/// JWT Service interface for generating and validating JWT tokens
/// </summary>
public interface IJwtService
{
    string GenerateAccessToken(string userId, string? email = null, string? phoneNumber = null);
    string GenerateRefreshToken();
    bool ValidateAccessToken(string token);
    string? GetUserIdFromToken(string token);
    int GetAccessTokenExpirationInMinutes();
    int GetRefreshTokenExpirationInDays();
}
