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
    List<UpdateProductImageRequest> Images,
    List<UpdateProductOptionRequest> Options,
    List<UpdateProductVariantRequest> Variants,
    UpdateProductVideoRequest? Video = null,
    // For simple products (no options/variants)
    decimal? DefaultPrice = null,
    int? DefaultStock = null,
    decimal? DefaultWeight = null,
    decimal? DefaultHeight = null,
    decimal? DefaultWidth = null,
    decimal? DefaultLength = null
);

/// <summary>
/// Request DTO for product image (update)
/// URL là identifier - giữ lại URL cũ để update metadata, thay đổi URL để thêm mới
/// </summary>
public record UpdateProductImageRequest(
    string ImageUrl,
    bool IsCover,
    int SortOrder
);

/// <summary>
/// Request DTO for product video (update)
/// URL là identifier - giữ lại URL cũ để giữ video, thay đổi URL để thay video
/// </summary>
public record UpdateProductVideoRequest(
    string VideoUrl
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
    string Sku,
    VariantStatus Status,
    Guid? Id = null, // null = new variant
    decimal? Weight = null,
    decimal? Height = null,
    decimal? Width = null,
    decimal? Length = null,
    List<UpdateProductImageRequest>? Images = null // Variant chỉ có Images, không có Video
);
