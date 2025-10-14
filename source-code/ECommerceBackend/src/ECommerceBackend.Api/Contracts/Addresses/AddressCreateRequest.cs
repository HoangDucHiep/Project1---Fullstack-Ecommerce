namespace ECommerceBackend.Api.Contracts.Addresses;

public sealed record AddressCreateRequest
(
    string Name,
    string Phone,
    string Province,
    string District,
    string Ward,
    string AddressLine,
    bool IsDefault,
    bool IsPickUpAddress,
    bool IsReturnAddress);
