using ECommerceBackend.Application.Abstracts.Sms;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.Extensions.Logging;

namespace ECommerceBackend.Infrastructure.Sms;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        try
        {
            // Mock implementation for development
            _logger.LogInformation("📱 Sending OTP {Otp} to phone {PhoneNumber}", otp, phoneNumber);

            // TODO: Production implementation
            // Example with Twilio:
            // var message = await _twilioClient.Messages.CreateAsync(
            //     to: new PhoneNumber(phoneNumber),
            //     from: new PhoneNumber(_configuration["Twilio:PhoneNumber"]),
            //     body: $"Your verification code is: {otp}");

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP to {PhoneNumber}", phoneNumber);
            return Task.FromResult(Result.Failure(SmsErrors.SendFailed));
        }
    }

    public Task<Result> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("📱 Sending SMS to {PhoneNumber}: {Message}", phoneNumber, message);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
            return Task.FromResult(Result.Failure(SmsErrors.SendFailed));
        }
    }
}
