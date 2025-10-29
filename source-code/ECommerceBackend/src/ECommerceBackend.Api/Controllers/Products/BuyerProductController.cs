using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Queries;
using ECommerceBackend.Application.Products.Queries.GetProductBySlug;
using ECommerceBackend.Application.Products.Queries.GetProductDetails;
using ECommerceBackend.Application.Products.Queries.GetProducts;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Products;

[ApiController]
[Route("api/v1/public/products")]
public class BuyerProductController : ControllerBase
{
    private readonly ISender _sender;

    public BuyerProductController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get product details by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductDetailsQuery(id);

        Result<ProductDetailDto> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy chi tiết sản phẩm thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    /// <summary>
    /// Get product details by slug
    /// </summary>
    /// <param name="slug">Product slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details</returns>
    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> GetProductBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductBySlugQuery(slug);

        Result<ProductDetailDto> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy chi tiết sản phẩm thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    /// <summary>
    /// Search products with filters and pagination (Public)
    /// </summary>
    /// <param name="q">Search text</param>
    /// <param name="categoryId">Filter by category</param>
    /// <param name="shopId">Filter by shop</param>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <param name="hasPromotion">Has promotion</param>
    /// <param name="pickupProvince">Pickup province</param>
    /// <param name="pickupDistrict">Pickup district</param>
    /// <param name="sortBy">Sort by</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated products</returns>
    [HttpGet]
    public async Task<IActionResult> SearchProductsAsync(
        [FromQuery] string? q = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? shopId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? hasPromotion = null,
        [FromQuery] string? pickupProvince = null,
        [FromQuery] string? pickupDistrict = null,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.Relevance,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(
            Q: q,
            CategoryId: categoryId,
            ShopId: shopId,
            Statuses: null, // Public không cho phép filter status
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasPromotion: hasPromotion,
            PickupProvince: pickupProvince,
            PickupDistrict: pickupDistrict,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize,
            AccessLevel: ProductAccessLevel.Public
        );

        Result<PaginationResult<ProductDto>> result = await _sender.Send(query, cancellationToken);

        object response = result.ToPaginatedResponse<ProductDto>("Tìm kiếm sản phẩm thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="q">Search text</param>
    /// <param name="shopId">Filter by shop</param>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <param name="hasPromotion">Has promotion</param>
    /// <param name="pickupProvince">Pickup province</param>
    /// <param name="pickupDistrict">Pickup district</param>
    /// <param name="sortBy">Sort by</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated products</returns>
    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetProductsByCategoryAsync(
        Guid categoryId,
        [FromQuery] string? q = null,
        [FromQuery] Guid? shopId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? hasPromotion = null,
        [FromQuery] string? pickupProvince = null,
        [FromQuery] string? pickupDistrict = null,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.Newest,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(
            Q: q,
            CategoryId: categoryId, // Fixed category
            ShopId: shopId,
            Statuses: null, // Public không cho phép filter status
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasPromotion: hasPromotion,
            PickupProvince: pickupProvince,
            PickupDistrict: pickupDistrict,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize,
            AccessLevel: ProductAccessLevel.Public
        );

        Result<PaginationResult<ProductDto>> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy danh sách sản phẩm theo danh mục thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    /// <summary>
    /// Get products by shop
    /// </summary>
    /// <param name="shopId">Shop ID</param>
    /// <param name="q">Search text</param>
    /// <param name="categoryId">Filter by category</param>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <param name="hasPromotion">Has promotion</param>
    /// <param name="pickupProvince">Pickup province</param>
    /// <param name="pickupDistrict">Pickup district</param>
    /// <param name="sortBy">Sort by</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated products</returns>
    [HttpGet("shop/{shopId:guid}")]
    public async Task<IActionResult> GetProductsByShopAsync(
        Guid shopId,
        [FromQuery] string? q = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? hasPromotion = null,
        [FromQuery] string? pickupProvince = null,
        [FromQuery] string? pickupDistrict = null,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.Newest,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(
            Q: q,
            CategoryId: categoryId,
            ShopId: shopId, // Fixed shop
            Statuses: null, // Public không cho phép filter status
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasPromotion: hasPromotion,
            PickupProvince: pickupProvince,
            PickupDistrict: pickupDistrict,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize,
            AccessLevel: ProductAccessLevel.Public
        );

        Result<PaginationResult<ProductDto>> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy danh sách sản phẩm theo shop thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }
}
