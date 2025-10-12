using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.LoginUserWithOtp;
public record LoginWithOtpCommand(
    string Identifier,
    string Password
) : ICommand;
