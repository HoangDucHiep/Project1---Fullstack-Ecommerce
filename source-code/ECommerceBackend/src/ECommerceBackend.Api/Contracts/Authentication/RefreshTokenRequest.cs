namespace ECommerceBackend.Api.Contracts.Authentication;

/// <summary>
/// Request model for refreshing access token
/// </summary>
/// <param name="RefreshToken">The refresh token to use for generating new access token</param>
public record RefreshTokenRequest(
    string RefreshToken
);
