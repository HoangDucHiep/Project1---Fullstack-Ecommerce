using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.Login;

public record LoginCommand(
    string Identifier, // Email or Phone
    string Password
) : ICommand<AuthenticationResult>;
