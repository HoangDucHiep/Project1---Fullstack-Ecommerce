using ECommerceBackend.Infrastructure.Identity;

namespace ECommerceBackend.Infrastructure.IdentityAuthen;
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public string? ReplacedByToken { get; set; }

    public string IdentityUserId { get; set; }
    public ApplicationIdentityUser IdentityUser { get; set; }


    public static RefreshToken Create(string token, string jwtId, DateTimeOffset expiresAtUtc, string identityUserId)
    {
        return new RefreshToken
        {
            Token = token,
            JwtId = jwtId,
            ExpiresAtUtc = expiresAtUtc,
            IdentityUserId = identityUserId,
            IsUsed = false,
            IsRevoked = false
        };
    }
}
