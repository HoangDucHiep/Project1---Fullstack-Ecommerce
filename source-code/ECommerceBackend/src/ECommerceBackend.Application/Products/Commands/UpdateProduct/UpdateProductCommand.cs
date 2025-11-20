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
    List<UpdateProductMediaDto> Medias,
    List<UpdateProductOptionDto> Options,
    List<UpdateProductVariantDto> Variants,
    // For simple products
    decimal? DefaultPrice = null,
    int? DefaultStock = null,
    double? DefaultWeight = null,
    double? DefaultHeight = null,
    double? DefaultWidth = null,
    double? DefaultLength = null
) : ICommand<ProductDetailDto>;

public record UpdateProductMediaDto(
    string? MediaUrl = null,
    Guid? Id = null,
    bool IsCover = false,
    int SortOrder = 0
);

public record UpdateProductOptionDto(
    Guid? Id = null,
    string Name = "",
    List<UpdateProductOptionValueDto>? Values = null
);

public record UpdateProductOptionValueDto(
    Guid? Id = null,
    string Value = ""
);

public record UpdateProductVariantDto(
    Guid? Id = null,
    List<string>? OptionValues = null,
    decimal Price = 0,
    int Stock = 0,
    string? Sku = null,
    double? Weight = null,
    double? Height = null,
    double? Width = null,
    double? Length = null,
    List<UpdateProductMediaDto>? Medias = null
);



