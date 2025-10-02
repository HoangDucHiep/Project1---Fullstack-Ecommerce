using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.RefreshTokenAsync(request.RefreshToken);
    }
}
