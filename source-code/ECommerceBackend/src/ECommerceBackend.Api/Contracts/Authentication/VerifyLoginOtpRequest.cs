namespace ECommerceBackend.Api.Contracts.Authentication;

/// <summary>
/// Request model for verifying login OTP
/// </summary>
/// <param name="Identifier">Email or phone number used for login</param>
/// <param name="Otp">The OTP code to verify</param>
public record VerifyLoginOtpRequest(
    string Identifier,
    string Otp
);
