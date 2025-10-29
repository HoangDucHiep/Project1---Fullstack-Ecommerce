using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProductsByCategory;

public sealed record GetProductsByCategoryQuery(
    Guid CategoryId,
    string? Q = null,                    // Text search
    Guid? ShopId = null,                 // Filter by shop
    decimal? MinPrice = null,            // Price range
    decimal? MaxPrice = null,            // Price range
    bool? HasPromotion = null,           // Has promotion
    string? PickupProvince = null,       // Filter by pickup location
    string? PickupDistrict = null,       // Filter by pickup location
    ProductSortBy SortBy = ProductSortBy.Newest, // Sorting
    int Page = 1,                        // Pagination
    int PageSize = 20                    // Pagination
) : IQuery<PaginationResult<ProductDto>>, IPaginableQuery;
