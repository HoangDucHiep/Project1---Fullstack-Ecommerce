using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.UpdateAddress;
internal sealed class UpdateAddressCommandHandler : ICommandHandler<UpdateAddressCommand, AddressDto>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAddressCommandHandler(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddressDto>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        // Find existing
        Address? existingAddress = await _addressRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingAddress is null)
        {
            return Result.Failure<AddressDto>(AddressErrors.NotFound());
        }

        // Check if the address belongs to the current user
        // TODO: Use UserContext to get current user ID and verify ownership
        var currentUserId = Guid.Parse("01998678-85b2-7474-883c-d17e816f46aa"); // Replace with actual user ID from context
        if (existingAddress.UserId != currentUserId)
        {
            return Result.Failure<AddressDto>(AddressErrors.Forbidden());
        }

        // Update fields
        existingAddress.Update(
            name: request.Name,
            phone: request.Phone,
            province: request.Province,
            district: request.District,
            ward: request.Ward,
            addressLine: request.AddressLine,
            isDefault: request.IsDefault,
            isPickUpAddress: request.IsPickUpAddress,
            isReturnAddress: request.IsReturnAddress
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(existingAddress.ToAddressDto());
    }
}
