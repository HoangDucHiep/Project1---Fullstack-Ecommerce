using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Sms;

/// HDHiep - 10/11/2025
/// <summary>
/// Service for sending SMS messages
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an OTP code via SMS
    /// </summary>
    Task<Result> SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a custom SMS message
    /// </summary>
    Task<Result> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
