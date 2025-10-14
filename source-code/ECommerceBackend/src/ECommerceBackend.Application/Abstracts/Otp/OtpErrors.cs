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
        $"Mã OTP không hợp lệ. Bạn còn {attemptsLeft} lần thử lại.");

    public static Error MaxAttemptsExceeded => Error.Validation(
        "Otp.MaxAttemptsExceeded",
        "Đã vượt quá số lần xác thực OTP tối đa.");

    public static Error ResendLimitReached(int secondLeft) => Error.Validation(
        "Otp.ResendLimitReached",
        $"Bạn đã quá nhiều yêu cầu, vui lòng thử lại sau {(secondLeft < 60 ? $"{secondLeft} giây" : $"{secondLeft / 60} phút")}");

    public static Error SessionExpired => Error.Validation(
        "Otp.SessionExpired",
        "Phiên OTP đã hết hạn. Vui lòng khởi động lại quá trình.");

    public static Error InvalidOtp => Error.Validation(
        "Otp.InvalidOtp",
        "Mã OTP được cung cấp không hợp lệ");

    public static Error OtpDelay(int delaySecs) => Error.Validation(
        "Otp.OtpDelay",
        $"Vui lòng thử lại sau {delaySecs} giây");
}
