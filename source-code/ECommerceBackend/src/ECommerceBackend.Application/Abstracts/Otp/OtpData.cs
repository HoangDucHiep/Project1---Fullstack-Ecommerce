namespace ECommerceBackend.Application.Abstracts.Otp;

/// <summary>
/// Represents OTP data stored in cache with metadata.
/// </summary>
/// <param name="Otp">The OTP code</param>
/// <param name="FailedAttempts">Number of failed verification attempts</param>
/// <param name="ResentCount">Number of times this OTP has been resent</param>
/// <param name="ExpiresAt">Expiration timestamp of the OTP</param>
public record OtpData(
    string Otp,
    int FailedAttempts,
    int ResentCount,
    DateTimeOffset ExpiresAt
);
