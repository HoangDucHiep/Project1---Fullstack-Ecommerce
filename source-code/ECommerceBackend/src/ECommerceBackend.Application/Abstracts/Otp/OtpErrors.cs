using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Otp;

/// HDHiep - 10/11/2025
/// <summary>
/// Common OTP-related errors
/// </summary>
public static class OtpErrors
{
    public static Error NotFound => Error.NotFound(
        "Otp.NotFound",
        "OTP session not found or expired");

    public static Error Expired => Error.Validation(
        "Otp.Expired",
        "OTP has expired");

    public static Error Invalid(int attemptsLeft) => Error.Validation(
        "Otp.Invalid",
        $"Invalid OTP code. {attemptsLeft} attempts left.");

    public static Error MaxAttemptsExceeded => Error.Validation(
        "Otp.MaxAttemptsExceeded",
        "Maximum OTP verification attempts exceeded");

    public static Error ResendLimitReached => Error.Validation(
        "Otp.ResendLimitReached",
        "Maximum OTP resend limit reached. Please restart the process.");

    public static Error SessionExpired => Error.Validation(
        "Otp.SessionExpired",
        "OTP session has expired. Please restart the process.");
}
