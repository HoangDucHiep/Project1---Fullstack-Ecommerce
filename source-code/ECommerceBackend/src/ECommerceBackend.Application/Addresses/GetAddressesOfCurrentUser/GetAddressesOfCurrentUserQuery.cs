using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Application.Contracts.Commons;

namespace ECommerceBackend.Application.Addresses.GetAddressesOfCurrentUser;

/// HDHiep - 10/01/2025
public sealed record GetAddressesOfCurrentUserQuery(
    int Page,
    int PageSize
    ) : IQuery<PaginationResult<AddressDto>>, IPaginableQuery;
