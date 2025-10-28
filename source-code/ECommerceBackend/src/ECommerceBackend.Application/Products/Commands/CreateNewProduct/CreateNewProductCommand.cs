using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Commands.CreateNewProduct;

public record CreateNewProductCommand(
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    string Sku, // Product SKU - required
    List<CreateProductMediaDto> Medias,
    List<CreateProductOptionDto> Options,
    List<CreateProductVariantDto> Variants,
    // For simple products (no options/variants)
    decimal? DefaultPrice = null,
    int? DefaultStock = null,
    double? DefaultWeight = null,
    double? DefaultHeight = null,
    double? DefaultWidth = null,
    double? DefaultLength = null
) : ICommand<ProductDetailDto>;

public record CreateProductMediaDto(
    string? MediaName = null,
    string? MediaUrl = null,
    bool IsCover = false,
    int SortOrder = 0
);

public record CreateProductOptionDto(
    string Name,
    List<string> Values
);

public record CreateProductVariantDto(
    List<string> OptionValues,
    decimal Price,
    int Stock,
    string? Sku = null, // Variant SKU (optional)
    decimal? Weight = null,
    decimal? Height = null,
    decimal? Width = null,
    decimal? Length = null,
    List<CreateProductMediaDto>? Medias = null
);
