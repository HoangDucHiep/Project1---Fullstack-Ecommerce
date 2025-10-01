using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Application.Contracts.Commons;

namespace ECommerceBackend.Application.Addresses.GetAddressesOfCurrentUser;
public sealed record GetAddressesOfCurrentUserQuery : IQuery<PaginationResult<AddressDto>>, IPaginableQuery
{
    public int Page { get; init; }
    public int PageSize { get; init; }
}
