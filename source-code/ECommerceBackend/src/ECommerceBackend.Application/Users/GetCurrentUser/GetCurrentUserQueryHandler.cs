using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Users;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Users.GetCurrentUser;

/// <summary>
/// Handler for getting current authenticated user information
/// </summary>
public sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserContext userContext, IUserRepository userRepository)
    {
        _userContext = userContext;
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAuthenticated)
        {
            return Result.Failure<UserDto>(new Error("User.NotAuthenticated", "User is not authenticated", ErrorType.Unauthorized));
        }

        string? identityUserId = _userContext.IdentityUserId;
        if (string.IsNullOrEmpty(identityUserId))
        {
            return Result.Failure<UserDto>(new Error("User.NotFound", "User ID not found in context", ErrorType.NotFound));
        }

        User? user = await _userRepository.GetByIdentityIdAsync(identityUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserDto>(new Error("User.NotFound", "User not found", ErrorType.NotFound));
        }

        return Result.Success(user.ToDto());
    }
}
