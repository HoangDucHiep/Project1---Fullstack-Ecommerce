namespace ECommerceBackend.Application.Authentication.Register;


/// <summary>
/// Registration session data stored separately from OTP
/// </summary>
public record RegistrationSession(
    string PhoneNumber,
    string PasswordHash,
    DateTimeOffset CreatedAt);
