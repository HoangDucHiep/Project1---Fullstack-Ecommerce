using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.RegisterUserWithOtp;

/// HDHiep - 10/12/2025
/// <summary>
/// Command to resend OTP for user registration.
/// </summary>
/// <param name="Phone"></param>
public record RegisterOtpResendCommand(string Phone) : ICommand<int>;
