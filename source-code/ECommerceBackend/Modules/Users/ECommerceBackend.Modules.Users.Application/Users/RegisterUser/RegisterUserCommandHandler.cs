using ECommerceBackend.Common.Application.Messaging;
using ECommerceBackend.Common.Domain;
using ECommerceBackend.Modules.Users.Application.Abstractions.Data;
using ECommerceBackend.Modules.Users.Domain.Users;

namespace ECommerceBackend.Modules.Users.Application.Users.RegisterUser;
internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{

    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(
            request.Email,
            request.FirstName,
            request.LastName
        );

        _userRepository.Insert(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
