namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Login session data stored separately from OTP.
/// Contains validated user information for the login process.
/// </summary>
public record LoginSession(
    string Identifier,
    string IdentityUserId,
    string? PhoneNumber,
    string? Email,
    DateTimeOffset CreatedAt);
