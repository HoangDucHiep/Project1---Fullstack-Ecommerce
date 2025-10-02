using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.Register;
public record RegisterEmployeeCommand(
    string Email,
    string Password
) : ICommand<AuthenticationResult>;
