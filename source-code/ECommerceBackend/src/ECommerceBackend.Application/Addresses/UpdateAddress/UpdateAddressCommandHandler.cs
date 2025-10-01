using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.UpdateAddress;
internal sealed class UpdateAddressCommandHandler : ICommandHandler<UpdateAddressCommandHandler, AddressDto>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAddress

    public Task<Result<AddressDto>> Handle(UpdateAddressCommandHandler request, CancellationToken cancellationToken)
    {
    }
}
