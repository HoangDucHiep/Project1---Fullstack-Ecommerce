using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Commons;

namespace ECommerceBackend.Domain.Products;


public class ProductMedia : Entity
{
    public Guid ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public string MediaUrl { get; private set; }
    public MediaType MediaType { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Initializes a new instance of the ProductMedia class.
    /// This constructor is required by Entity Framework Core for object materialization.
    /// </summary>
    public ProductMedia()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Creates a new instance of ProductMedia.
    /// </summary>
    /// <param name="productId">The unique identifier of the product this media belongs to.</param>
    /// <param name="mediaUrl">The URL or path to the media file.</param>
    /// <param name="mediaType">The type of media (Image, Video, Document, or Other).</param>
    /// <param name="sortOrder">The display order of this media relative to other media for the same product.</param>
    /// <returns>A new ProductMedia instance configured for a product.</returns>
    public static ProductMedia CreateProductMedia(Guid productId, string mediaUrl, MediaType mediaType, int sortOrder)
    {
        return new ProductMedia
        {
            ProductId = productId,
            ProductVariantId = null,
            MediaUrl = mediaUrl,
            MediaType = mediaType,
            SortOrder = sortOrder,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates a new instance of ProductVariantMedia.
    /// </summary>
    /// <param name="productId">The unique identifier of the product this media belongs to.</param>
    /// <param name="productVariantId">The unique identifier of the product variant this media is associated with.</param>
    /// <param name="mediaUrl">The URL or path to the media file.</param>
    /// <param name="mediaType">The type of media (Image, Video, Document, or Other).</param>
    /// <param name="sortOrder">The display order of this media relative to other media for the same product.</param>
    /// <returns></returns>
    public static ProductMedia CreateProductVariantMedia(Guid productId, Guid productVariantId, string mediaUrl, MediaType mediaType, int sortOrder)
    {
        return new ProductMedia
        {
            ProductId = productId,
            ProductVariantId = productVariantId,
            MediaUrl = mediaUrl,
            MediaType = mediaType,
            SortOrder = sortOrder,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
