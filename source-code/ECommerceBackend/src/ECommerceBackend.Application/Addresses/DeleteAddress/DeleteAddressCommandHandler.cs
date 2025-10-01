using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.DeleteAddress;
internal sealed class DeleteAddressCommandHandler : ICommandHandler<DeleteAddressCommand>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAddressCommandHandler(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        Address? address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
        if (address is null)
        {
            return Result.Failure(AddressErrors.NotFound());
        }

        // Check if the address belongs to the current user
        // TODO: Use UserContext to get current user ID and verify ownership
        // For demonstration, assuming a fixed user ID
        var currentUserId = Guid.Parse("01998678-85b2-7474-883c-d17e816f46aa"); // Replace with actual user ID from context

        if (address.UserId != currentUserId)
        {
            return Result.Failure(AddressErrors.Forbidden());
        }

        _addressRepository.Delete(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
