namespace ECommerceBackend.Api.Contracts.Addresses;
public sealed record AddressUpdateRequest
(
    string Name,
    string Phone,
    string Province,
    string District,
    string Ward,
    string AddressLine,
    bool IsDefault,
    bool IsPickUpAddress,
    bool IsReturnAddress
);
