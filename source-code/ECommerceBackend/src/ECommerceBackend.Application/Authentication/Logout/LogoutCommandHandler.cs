using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Authentication.Logout;


public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserContext _userContext;

    public LogoutCommandHandler(IAuthenticationService authenticationService, IUserContext userContext)
    {
        _authenticationService = authenticationService;
        _userContext = userContext;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAuthenticated || string.IsNullOrEmpty(_userContext.UserId))
        {
            return Result.Failure(new Error("Unauthorized", "User is not authenticated", ErrorType.Unauthorized));
        }

        return await _authenticationService.LogoutAsync(_userContext.UserId);
    }
}
