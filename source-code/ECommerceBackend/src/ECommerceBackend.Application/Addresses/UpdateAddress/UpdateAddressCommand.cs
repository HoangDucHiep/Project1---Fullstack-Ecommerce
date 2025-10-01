using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;

namespace ECommerceBackend.Application.Addresses.UpdateAddress;
public sealed record UpdateAddressCommand(
    string Name,
    string Phone,
    string Province,
    string District,
    string Ward,
    string AddressLine,
    bool IsDefault,
    bool IsPickUpAddress,
    bool IsReturnAddress
) : ICommand<AddressDto>;
