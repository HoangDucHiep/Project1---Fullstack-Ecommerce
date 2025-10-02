using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.Login;
public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.LoginAsync(request.Identifier, request.Password);
    }
}
