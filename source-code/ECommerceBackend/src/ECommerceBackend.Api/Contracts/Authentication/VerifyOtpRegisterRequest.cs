namespace ECommerceBackend.Api.Contracts.Authentication;

public record VerifyOtpRegisterRequest(
    string phoneNumber,
    string otp);
