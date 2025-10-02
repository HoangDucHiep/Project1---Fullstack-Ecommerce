using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.Register;

public class RegisterEmployeeCommandHandler : ICommandHandler<RegisterEmployeeCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;

    public RegisterEmployeeCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.RegisterEmployeeAsync(request.Email, request.Password);
    }
}
