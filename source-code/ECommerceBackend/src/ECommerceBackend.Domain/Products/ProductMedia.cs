using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Medias;

namespace ECommerceBackend.Domain.Products;
public class ProductMedia : Entity
{
    public Guid ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public Guid MediaId { get; private set; }
    public bool IsCover { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    // Navigation properties
    public Product Product { get; private set; } = null!;
    public ProductVariant? ProductVariant { get; private set; }
    public Media Media { get; private set; } = null!;

    private ProductMedia() { } // For EF Core

    public static ProductMedia CreateForProduct(
        Guid productId,
        Guid mediaId,
        bool isCover = false,
        int sortOrder = 0)
    {
        return new ProductMedia
        {
            ProductId = productId,
            ProductVariantId = null,
            MediaId = mediaId,
            IsCover = isCover,
            SortOrder = sortOrder,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public static ProductMedia CreateForVariant(
        Guid productId,
        Guid productVariantId,
        Guid mediaId,
        bool isCover = false,
        int sortOrder = 0)
    {
        return new ProductMedia
        {
            ProductId = productId,
            ProductVariantId = productVariantId,
            MediaId = mediaId,
            IsCover = isCover,
            SortOrder = sortOrder,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void UpdateCoverStatus(bool isCover)
    {
        IsCover = isCover;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
