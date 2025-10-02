using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.Register;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;

    public RegisterUserCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.RegisterUserAsync(
            request.PhoneNumber,
            request.Email,
            request.Password);
    }
}
