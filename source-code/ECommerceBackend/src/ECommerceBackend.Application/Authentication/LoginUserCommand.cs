using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication;

/// HDHiep - 10/10/2025
/// <summary>
/// Command to login user with identifier (email or phone) and password
/// </summary>
/// <param name="Identifier">Email or phone number</param>
/// <param name="Password">User password</param>
public record LoginUserCommand(
    string Identifier,
    string Password
) : ICommand<AuthenticationResult>;
