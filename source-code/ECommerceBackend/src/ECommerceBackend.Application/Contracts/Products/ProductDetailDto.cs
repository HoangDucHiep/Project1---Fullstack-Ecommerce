using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Contracts.Products;

public record ProductDetailDto(
    Guid Id,
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Slug,
    string Description,
    ProductStatus Status,
    List<ProductMediaDto> Media,
    List<ProductOptionDto> Options,
    List<ProductVariantDetailDto> Variants,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc
);

public record ProductVariantDetailDto(
    Guid Id,
    List<string> OptionValues,
    decimal Price,
    int Stock,
    string Sku,
    double Weight,
    double Height,
    double Width,
    double Length,
    List<ProductMediaDto>? VariantMedia = null
);
