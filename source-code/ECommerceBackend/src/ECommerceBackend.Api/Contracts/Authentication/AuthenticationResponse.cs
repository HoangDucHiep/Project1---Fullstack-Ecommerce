namespace ECommerceBackend.Api.Contracts.Authentication;

public record AuthenticationResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTimeOffset AccessTokenExpiration { get; init; }
    public DateTimeOffset RefreshTokenExpiration { get; init; }
    public string IdentityUserId { get; init; } = string.Empty;
}
