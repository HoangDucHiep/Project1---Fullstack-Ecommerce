namespace ECommerceBackend.Application.Contracts.Products;

/// <summary>
/// DTO for product media (images and videos)
/// </summary>
public record ProductMediaDto(
    string MediaUrl,
    string MediaType,
    int SortOrder,
    bool IsCover = false
);

