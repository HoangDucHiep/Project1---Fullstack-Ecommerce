using ECommerceBackend.Common.Application.Messaging;

namespace ECommerceBackend.Modules.Users.Application.Users.RegisterUser;

/// HoangDucHiep - 08/2025
/// <summary>
/// Command to register a new user with phone and password.
/// </summary>
public sealed record RegisterUserCommand(string Phone, string Password) : ICommand<Guid>;
