using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Sms;


/// HDHiep - 10/11/2025
/// <summary>
/// Common SMS-related errors
/// </summary>
public static class SmsErrors
{
    public static Error SendFailed => Error.Failure(
        "Sms.SendFailed",
        "Failed to send SMS");

    public static Error InvalidPhoneNumber => Error.Validation(
        "Sms.InvalidPhoneNumber",
        "Invalid phone number format");

    public static Error ProviderError(string details) => Error.Failure(
        "Sms.ProviderError",
        $"SMS provider error: {details}");
}
