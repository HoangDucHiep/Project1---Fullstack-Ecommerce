namespace ECommerceBackend.Application.Abstracts.Authentication;

public interface IJwtService
{
    string GenerateAccessToken(string userId, string email, string? phoneNumber = null);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    string? GetUserIdFromToken(string token);
}
