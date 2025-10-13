using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication;

/// HDHiep - 10/10/2025
/// <summary>
/// Handler for login user command
/// </summary>
public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginUserCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.InternalLoginAsync(request.Identifier, request.Password);
    }
}
