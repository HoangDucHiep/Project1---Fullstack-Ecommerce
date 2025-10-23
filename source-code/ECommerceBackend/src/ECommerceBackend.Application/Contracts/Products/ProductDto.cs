namespace ECommerceBackend.Application.Contracts.Products;

/// <summary>
/// DTO for product information
/// </summary>
public record ProductDto(
    Guid Id,
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    string Slug,
    string Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc
);

