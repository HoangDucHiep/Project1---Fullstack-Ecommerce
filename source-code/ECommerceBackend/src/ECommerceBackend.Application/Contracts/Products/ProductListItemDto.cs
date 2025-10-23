namespace ECommerceBackend.Application.Contracts.Products;

public record ProductListItemDto(
    Guid Id,
    string Name,
    string Slug,
    decimal MinPrice,
    decimal MaxPrice,
    string CoverImageUrl,
    Guid ShopId,        // Frontend sẽ query riêng
    Guid CategoryId,    // Frontend sẽ query riêng
    bool HasDiscount,
    decimal? DiscountPercentage,
    DateTimeOffset CreatedAtUtc
);
