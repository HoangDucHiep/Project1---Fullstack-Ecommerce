using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication;

/// <summary>
/// Command to refresh access token using a valid refresh token
/// </summary>
/// <param name="RefreshToken">The refresh token to use for generating new access token</param>
public record RefreshTokenCommand(
    string RefreshToken
) : ICommand<AuthenticationResult>;
