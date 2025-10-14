using ECommerceBackend.Api.Contracts.Commons;

namespace ECommerceBackend.Api.Contracts.Addresses;

public record GetAddressOfCurrentUserRequest : IPaginableRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
