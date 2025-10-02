using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.RefreshToken;
public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthenticationResult>;
