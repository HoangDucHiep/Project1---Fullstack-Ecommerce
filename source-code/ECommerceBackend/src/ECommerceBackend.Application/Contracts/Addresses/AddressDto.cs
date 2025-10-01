namespace ECommerceBackend.Application.Contracts.Addresses;

/// HDHiep - 10/01/2025
/// <summary>
/// Address Data Transfer Object (DTO)
/// </summary>
public sealed record AddressDto // : ILinksResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Phone { get; init; }
    public string Province { get; init; }
    public string District { get; init; }
    public string Ward { get; init; }
    public string AddressLine { get; init; }
    public bool IsDefault { get; init; }
    public bool IsPickUpAddress { get; init; }
    public bool IsReturnAddress { get; init; }
}

