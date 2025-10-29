using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProducts;

/// <summary>
/// Query to get products with various filters
/// </summary>
public sealed record GetProductsQuery(
    string? Q = null,                    // Text search
    Guid? CategoryId = null,             // Filter by category
    Guid? ShopId = null,                 // Filter by shop
    List<ProductStatus>? Statuses = null, // Filter by multiple product statuses
    decimal? MinPrice = null,            // Price range
    decimal? MaxPrice = null,            // Price range
    bool? HasPromotion = null,           // Has promotion
    string? PickupProvince = null,       // Filter by pickup location
    string? PickupDistrict = null,       // Filter by pickup location
    ProductSortBy SortBy = ProductSortBy.Newest, // Sorting
    int Page = 1,                        // Pagination
    int PageSize = 20,                   // Pagination
    ProductAccessLevel AccessLevel = ProductAccessLevel.Public // Access level
) : IQuery<PaginationResult<ProductDto>>, IPaginableQuery;
