using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    Guid CategoryId,
    string Name,
    string Description,
    string Sku,
    ProductStatus Status,
    List<UpdateProductImageDto> Images,
    List<UpdateProductOptionDto> Options,
    List<UpdateProductVariantDto> Variants,
    UpdateProductVideoDto? Video = null,
    // For simple products
    decimal? DefaultPrice = null,
    int? DefaultStock = null,
    double? DefaultWeight = null,
    double? DefaultHeight = null,
    double? DefaultWidth = null,
    double? DefaultLength = null
) : ICommand<ProductDetailDto>;

public record UpdateProductImageDto(
    string ImageUrl,
    bool IsCover,
    int SortOrder,
    Guid? Id = null
);

public record UpdateProductVideoDto(
    string VideoUrl,
    Guid? Id = null
);

public record UpdateProductOptionDto(
    string Name,
    List<UpdateProductOptionValueDto> Values,
    Guid? Id = null
);

public record UpdateProductOptionValueDto(
    string Value,
    Guid? Id = null
);

public record UpdateProductVariantDto(
    List<string> OptionValues,
    decimal Price,
    int Stock,
    string Sku,
    Guid? Id = null,
    double? Weight = null,
    double? Height = null,
    double? Width = null,
    double? Length = null,
    List<UpdateProductImageDto>? Images = null
);
