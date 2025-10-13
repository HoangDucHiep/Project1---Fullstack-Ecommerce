using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;

/// <summary>
/// Command to resend OTP during user login process.
/// Regenerates and sends a new OTP for login verification.
/// </summary>
/// <param name="Identifier">The identifier (phone number or email) for login</param>
public record LoginOtpResendCommand(
    string Identifier
) : ICommand<int>;
