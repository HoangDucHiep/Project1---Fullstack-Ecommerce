using ECommerceBackend.Application.Contracts.Media;

namespace ECommerceBackend.Application.Contracts.Products;



/// <summary>
/// DTO for product media (images and videos)
/// </summary>
public record ProductMediaDto(
    Guid Id,
    Guid ProductId,
    Guid? ProductVariantId,
    Guid MediaId,
    bool IsCover,
    int SortOrder,
    DateTimeOffset CreatedAtUtc,
    MediaDto Media
)
{
    /// <summary>
    /// Factory method to create ProductMediaDto from ProductMedia entity
    /// </summary>
    public static ProductMediaDto FromEntity(Domain.Products.ProductMedia productMedia) => new(
        productMedia.Id,
        productMedia.ProductId,
        productMedia.ProductVariantId,
        productMedia.MediaId,
        productMedia.IsCover,
        productMedia.SortOrder,
        productMedia.CreatedAtUtc,
        MediaDto.FromEntity(productMedia.Media)
    );
};

