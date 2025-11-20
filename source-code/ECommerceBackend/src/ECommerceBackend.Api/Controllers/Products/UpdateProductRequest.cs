using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Api.Controllers.Products;

/// <summary>
/// Request DTO for updating a product
/// </summary>
public record UpdateProductRequest(
    Guid CategoryId,
    string Name,
    string Description,
    string Sku,
    ProductStatus Status,
    List<UpdateProductMediaRequest> Medias,
    List<UpdateProductOptionRequest> Options,
    List<UpdateProductVariantRequest> Variants,
    // For simple products (no options/variants)
    decimal? DefaultPrice = null,
    int? DefaultStock = null,
    decimal? DefaultWeight = null,
    decimal? DefaultHeight = null,
    decimal? DefaultWidth = null,
    decimal? DefaultLength = null
);

/// <summary>
/// Request DTO for product media (update)
/// </summary>
public record UpdateProductMediaRequest(
    string? MediaUrl = null,
    Guid? Id = null, // null = new media
    bool IsCover = false,
    int SortOrder = 0
);

/// <summary>
/// Request DTO for product option (update)
/// </summary>
public record UpdateProductOptionRequest(
    string Name,
    List<UpdateProductOptionValueRequest> Values,
    Guid? Id = null // null = new option
);

/// <summary>
/// Request DTO for product option value (update)
/// </summary>
public record UpdateProductOptionValueRequest(
    string Value,
    Guid? Id = null // null = new value
);

/// <summary>
/// Request DTO for product variant (update)
/// </summary>
public record UpdateProductVariantRequest(
    List<string> OptionValues,
    decimal Price,
    int Stock,
    Guid? Id = null, // null = new variant
    string? Sku = null,
    decimal? Weight = null,
    decimal? Height = null,
    decimal? Width = null,
    decimal? Length = null,
    List<UpdateProductMediaRequest>? Medias = null
);

