using ECommerceBackend.Common.Application.Messaging;
using ECommerceBackend.Common.Domain;
using ECommerceBackend.Modules.Users.Application.Abstractions.Data;
using ECommerceBackend.Modules.Users.Application.Identity;
using ECommerceBackend.Modules.Users.Domain.Users;

namespace ECommerceBackend.Modules.Users.Application.Users.RegisterUser;

/// HoangDucHiep-08/2025
/// <summary>
/// Handler for the <see cref="RegisterUserCommand"/> command.
/// Handles user registration by interacting with the identity provider service
/// and the user repository.
/// Returns the ID of the newly registered user upon success.
/// Implements <see cref="ICommandHandler{TCommand, TResult}"/>.
/// Takes <see cref="RegisterUserCommand"/> as input and returns a <see cref="Guid"/> representing the user ID.
/// Handles failures by returning appropriate error messages.
/// Uses <see cref="IIdentityProviderService"/> to register the user in the identity provider.
/// Uses <see cref="IUserRepository"/> to persist the user entity.
/// Uses <see cref="IUnitOfWork"/> to commit changes to the database.
/// Handles cancellation via <see cref="CancellationToken"/>.
/// </summary>
internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{

    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityProviderService _identityProviderService;

    public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityProviderService identityProviderService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _identityProviderService = identityProviderService;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Result<string> result = await _identityProviderService.RegisterUserAsync(new UserModel(request.Phone, request.Password), cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        var user = User.Create(
            request.Phone,
            result.Value
        );

        _userRepository.Insert(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
