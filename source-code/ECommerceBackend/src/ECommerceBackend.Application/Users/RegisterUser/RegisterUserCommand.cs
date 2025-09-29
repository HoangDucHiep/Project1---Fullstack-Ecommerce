using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Users.RegisterUser;

/// HDHiep - 09/24/2025
/// <summary>
/// Command to register a new user
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string Password) : ICommand<Guid>;
