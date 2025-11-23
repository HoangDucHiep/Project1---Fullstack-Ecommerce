using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Products;

/// HDHiep - 10/21/2025
/// <summary>
/// Static class containing error definitions for product-related operations.
/// </summary>
public static class ProductErrors
{
    public static Error NotFound(Guid productId) => Error.NotFound(
        "Product.NotFound",
        $"Product with ID '{productId}' was not found");

    public static Error ShopIdRequiredForSellerView() => Error.Validation(
        "Product.ShopIdRequiredForSellerView",
        "ShopId is required when accessing products as a seller");

    public static Error NotFound(string slug) => Error.NotFound(
        "Product.NotFound",
        $"Product with Slug '{slug}' was not found");
    public static Error SlugAlreadyExists(string slug) => Error.Conflict(
        "Product.SlugAlreadyExists",
        $"A product with slug '{slug}' already exists");

    public static Error InvalidName(string reason) => Error.Validation(
        "Product.InvalidName",
        $"Product name is invalid: {reason}");

    public static Error InvalidDescription(string reason) => Error.Validation(
        "Product.InvalidDescription",
        $"Product description is invalid: {reason}");

    public static Error ShopNotFound(Guid shopId) => Error.NotFound(
        "Product.ShopNotFound",
        $"Shop with ID '{shopId}' was not found");

    public static Error CategoryNotFound(Guid categoryId) => Error.NotFound(
        "Product.CategoryNotFound",
        $"Category with ID '{categoryId}' was not found");

    public static Error TooManyMedia(int count, int max) => Error.Validation(
        "Product.TooManyMedia",
        $"Too many media files. Maximum is {max}, but received {count}");

    public static Error TooManyVideos(int count, int max) => Error.Validation(
        "Product.TooManyVideos",
        $"Too many videos. Maximum is {max}, but received {count}");

    public static Error TooManyOptions(int count, int max) => Error.Validation(
        "Product.TooManyOptions",
        $"Too many product options. Maximum is {max}, but received {count}");

    public static Error OptionWithoutValues(string optionName) => Error.Validation(
        "Product.OptionWithoutValues",
        $"Product option '{optionName}' must have at least one value");

    public static Error DuplicateOptionName(string optionName) => Error.Validation(
        "Product.DuplicateOptionName",
        $"Product option '{optionName}' is duplicated");

    public static Error DuplicateSku(string sku) => Error.Conflict(
        "Product.DuplicateSku",
        $"A product variant with SKU '{sku}' already exists");

    public static Error SkuDuplicate(string sku) => Error.Conflict(
        "Product.SkuDuplicate",
        $"SKU '{sku}' đã tồn tại trong shop này");

    public static Error VariantSkuDuplicate(string sku) => Error.Conflict(
        "Variant.SkuDuplicate",
        $"Variant SKU '{sku}' đã tồn tại trong shop này");

    public static Error VariantSkuDuplicateInRequest(string duplicates) => Error.Validation(
        "Variant.SkuDuplicateInRequest",
        $"Variant SKU trùng lặp trong request: {duplicates}");

    public static Error InvalidVariantCount(int expected, int actual) => Error.Validation(
        "Product.InvalidVariantCount",
        $"Number of variants ({actual}) does not match the expected combinations ({expected})");

    public static Error InvalidOptionValue(string optionValue, string optionName) => Error.Validation(
        "Product.InvalidOptionValue",
        $"Option value '{optionValue}' is not defined in option '{optionName}'");

    public static Error InvalidPrice(decimal price) => Error.Validation(
        "Product.InvalidPrice",
        $"Price must be greater than 0, but received {price}");

    public static Error InvalidStock(int stock) => Error.Validation(
        "Product.InvalidStock",
        $"Stock must be greater than or equal to 0, but received {stock}");

    public static Error InvalidWeight(double weight) => Error.Validation(
        "Product.InvalidWeight",
        $"Weight must be greater than 0, but received {weight}");

    public static Error InvalidDimensions(string reason) => Error.Validation(
        "Product.InvalidDimensions",
        $"Product dimensions are invalid: {reason}");

    public static Error NoVariants() => Error.Validation(
        "Product.NoVariants",
        "Product must have at least one variant");

    public static Error NoMedia() => Error.Validation(
        "Product.NoMedia",
        "Product must have at least one image");

    public static Error MediaNotFound(string mediaUrl) => Error.NotFound(
        "Product.MediaNotFound",
        $"Media file '{mediaUrl}' was not found");

    public static Error MultipleCoverImages() => Error.Validation(
        "Product.MultipleCoverImages",
        "Product cannot have more than one cover image");

    public static Error NoCoverImage() => Error.Validation(
        "Product.NoCoverImage",
        "Product must have at least one cover image");

    public static Error VariantMultipleCoverImages(string sku) => Error.Validation(
        "Product.VariantMultipleCoverImages",
        $"Variant '{sku}' cannot have more than one cover image");

    public static Error Unauthorized => Error.Forbidden(
        "Product.Unauthorized",
        "Bạn không có quyền cập nhật sản phẩm này");

    public static Error ProductLocked => Error.Failure(
        "Product.Locked",
        "Không thể cập nhật sản phẩm đang bị khóa bởi Admin");

    public static Error InvalidStatusTransition => Error.Validation(
        "Product.InvalidStatusTransition",
        "Trạng thái không hợp lệ. Seller chỉ có thể chuyển giữa Active, Inactive, OutOfStock");

    public static Error OptionNotFound(Guid optionId) => Error.NotFound(
        "Product.OptionNotFound",
        $"Product option with ID '{optionId}' was not found");

    public static Error OptionValueNotFound(Guid valueId) => Error.NotFound(
        "Product.OptionValueNotFound",
        $"Product option value with ID '{valueId}' was not found");

    public static Error VariantNotFound(Guid variantId) => Error.NotFound(
        "Product.VariantNotFound",
        $"Product variant with ID '{variantId}' was not found");
}

