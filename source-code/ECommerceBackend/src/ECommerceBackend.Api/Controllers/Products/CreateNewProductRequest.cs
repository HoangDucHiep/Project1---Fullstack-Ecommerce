namespace ECommerceBackend.Api.Controllers.Products;

/// <summary>
/// Request DTO for creating a new product
/// </summary>
public record CreateNewProductRequest(
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    string Sku, // Product SKU - required
    List<CreateProductImageRequest> Images, // Tối đa 10 ảnh
    CreateProductVideoRequest? Video, // Tối đa 1 video (optional)
    List<CreateProductOptionRequest> Options,
    List<CreateProductVariantRequest> Variants,
    // For simple products (no options/variants)
    decimal? DefaultPrice = null,
    int? DefaultStock = null,
    decimal? DefaultWeight = null,
    decimal? DefaultHeight = null,
    decimal? DefaultWidth = null,
    decimal? DefaultLength = null
);

/// <summary>
/// Request DTO for product image
/// </summary>
public record CreateProductImageRequest(
    string ImageUrl,
    bool IsCover = false,
    int SortOrder = 0
);

/// <summary>
/// Request DTO for product video
/// </summary>
public record CreateProductVideoRequest(
    string VideoUrl,
    int SortOrder = 0
);

/// <summary>
/// Request DTO for product option
/// </summary>
public record CreateProductOptionRequest(
    string Name,
    List<string> Values
);

/// <summary>
/// Request DTO for product variant
/// </summary>
public record CreateProductVariantRequest(
    List<string> OptionValues,
    decimal Price,
    int Stock,
    string? Sku = null, // Variant SKU (optional)
    decimal? Weight = null,
    decimal? Height = null,
    decimal? Width = null,
    decimal? Length = null,
    List<CreateProductImageRequest>? Images = null // Tối đa 3 ảnh
);

/// <summary>
/// Request DTO for variant image
/// </summary>
public record CreateVariantImageRequest(
    string ImageUrl,
    bool IsCover = false,
    int SortOrder = 0
);
