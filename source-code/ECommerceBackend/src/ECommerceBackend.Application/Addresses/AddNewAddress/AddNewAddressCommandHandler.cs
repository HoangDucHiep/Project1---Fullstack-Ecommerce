using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.AddNewAddress;
public sealed class AddNewAddressCommandHandler : ICommandHandler<AddNewAddressCommand, AddressDto>
{

    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    // TODO: Need to get UserContext later

    public AddNewAddressCommandHandler(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddressDto>> Handle(AddNewAddressCommand request, CancellationToken cancellationToken)
    {
        // TODO: Get user ID from context

        var newAdress = Address.Create(
            userId: Guid.Parse("01998678-85b2-7474-883c-d17e816f46aa"), // TODO: Replace with actual user ID from context
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

        _addressRepository.Add(newAdress);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var addressDto = newAdress.ToAddressDto();

        return addressDto;
    }
}
