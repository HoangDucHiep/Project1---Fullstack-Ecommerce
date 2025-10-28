using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Queries.SearchProducts;


/// HDHiep - 2025/10/29
/// <summary>
/// Search products with various filters and sorting options
/// </summary>
/// <param name="Q"></param>
/// <param name="CategoryId"></param>
/// <param name="ShopId"></param>
/// <param name="MinPrice"></param>
/// <param name="MaxPrice"></param>
/// <param name="HasPromotion"></param>
/// <param name="PickupProvince"></param>
/// <param name="PickupDistrict"></param>
/// <param name="SortBy"></param>
/// <param name="Page"></param>
/// <param name="PageSize"></param>
public sealed record SearchProductsQuery(
    string? Q = null,                    // Text search
    Guid? CategoryId = null,             // Filter by category
    Guid? ShopId = null,                 // Filter by shop
    decimal? MinPrice = null,            // Price range
    decimal? MaxPrice = null,            // Price range
    bool? HasPromotion = null,           // Has promotion (tạm thời false)
    string? PickupProvince = null,       // Filter by pickup location
    string? PickupDistrict = null,       // Filter by pickup location
    ProductSortBy SortBy = ProductSortBy.Relevance, // Sorting
    int Page = 1,                        // Pagination
    int PageSize = 20                    // Pagination
) : IQuery<PaginationResult<ProductDto>>, IPaginableQuery;

public enum ProductSortBy
{
    Relevance,    // Default when Q is provided
    Newest,       // CreatedAtUtc DESC
    PriceAsc,     // Min price ASC
    PriceDesc     // Min price DESC
}
