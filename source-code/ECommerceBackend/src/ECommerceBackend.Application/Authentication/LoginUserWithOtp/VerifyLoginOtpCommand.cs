using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Command to verify OTP during user login process.
/// Completes the login by verifying the OTP and returning authentication tokens.
/// </summary>
/// <param name="Identifier">The identifier (phone number or email) used for login</param>
/// <param name="Otp">The OTP code to verify</param>
public record VerifyLoginOtpCommand(
    string Identifier,
    string Otp
) : ICommand<AuthenticationResult>;
