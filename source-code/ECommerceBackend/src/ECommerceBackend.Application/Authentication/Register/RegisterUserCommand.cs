using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.Register;
public record RegisterUserCommand(
    string PhoneNumber,
    string? Email = null,
    string? Password = null
) : ICommand<AuthenticationResult>;
