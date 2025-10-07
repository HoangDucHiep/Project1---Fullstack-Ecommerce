using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication;


/// HDHiep - 10/06/2025
/// <summary>
/// Command to register a new user with phone number and optional password
/// </summary>
/// <param name="PhoneNumber"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record RegisterUserCommand(
    string PhoneNumber,
    string? Password = null
) : ICommand<AuthenticationResult>;
