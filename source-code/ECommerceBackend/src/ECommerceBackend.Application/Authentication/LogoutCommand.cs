using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication;

/// <summary>
/// Command to logout user and revoke all refresh tokens
/// </summary>
/// <param name="UserId">The user ID to logout</param>
public record LogoutCommand(
    string UserId
) : ICommand;
