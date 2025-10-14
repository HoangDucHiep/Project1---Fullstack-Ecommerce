using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.RegisterUserWithOtp;


public record VerifyRegistrationOtpCommand(string PhoneNumber, string Otp) : ICommand<AuthenticationResult>;
