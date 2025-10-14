using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.DeleteAddress;
public sealed class DeleteAddressCommandHandler : ICommandHandler<DeleteAddressCommand>
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

        if (address is null || address.UserId != request.UserId)
        {
            return Result.Failure(AddressErrors.NotFound());
        }

        if (address.IsDefault || address.IsPickUpAddress || address.IsReturnAddress)
        {
            return Result.Failure(AddressErrors.ReplacementRequired());
        }

        _addressRepository.Delete(address);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
