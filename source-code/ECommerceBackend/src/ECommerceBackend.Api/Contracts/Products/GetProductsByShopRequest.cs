using ECommerceBackend.Application.Products.Queries;

namespace ECommerceBackend.Api.Controllers.Products;

/// HDHiep - 2025/10/29
/// <summary>
/// Request DTO for getting products by shop
/// </summary>
public record GetProductsByShopRequest(
    string? Q = null,                    // Text search
    Guid? CategoryId = null,             // Filter by category
    decimal? MinPrice = null,            // Price range
    decimal? MaxPrice = null,            // Price range
    bool? HasPromotion = null,           // Has promotion
    string? PickupProvince = null,       // Filter by pickup location
    string? PickupDistrict = null,       // Filter by pickup location
    ProductSortBy SortBy = ProductSortBy.Newest, // Sorting
    int Page = 1,                        // Pagination
    int PageSize = 20                    // Pagination
);
