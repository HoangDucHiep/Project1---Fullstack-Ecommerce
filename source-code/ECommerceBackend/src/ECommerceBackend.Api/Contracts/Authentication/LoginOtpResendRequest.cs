namespace ECommerceBackend.Api.Contracts.Authentication;

/// <summary>
/// Request model for resending login OTP
/// </summary>
/// <param name="Identifier">Email or phone number used for login</param>
public record LoginOtpResendRequest(
    string Identifier
);
