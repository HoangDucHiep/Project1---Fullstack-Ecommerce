using Microsoft.AspNetCore.Identity;

namespace ECommerceBackend.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public override string? PhoneNumber { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Refresh token properties
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
