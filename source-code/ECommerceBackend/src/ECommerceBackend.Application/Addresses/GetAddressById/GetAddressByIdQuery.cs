using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;

namespace ECommerceBackend.Application.Addresses.GetAddressById;
public sealed record GetAddressByIdQuery(
    Guid AddressId
    ) : IQuery<AddressDto>;
