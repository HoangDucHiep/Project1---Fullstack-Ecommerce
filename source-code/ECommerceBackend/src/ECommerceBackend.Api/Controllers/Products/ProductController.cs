using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Commands.CreateProduct;
using ECommerceBackend.Application.Products.Queries.GetProduct;
using ECommerceBackend.Application.Products.Queries.GetProducts;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Products;

/// <summary>
/// Controller for product-related operations
/// </summary>
[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        ISender sender,
        ILogger<ProductController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Get products list with filtering, sorting and pagination (Public view for customers)
    /// </summary>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="hasDiscount">Filter products with discount</param>
    /// <param name="shopId">Filter by shop ID</param>
    /// <param name="sortBy">Sort order</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? hasDiscount = null,
        [FromQuery] Guid? shopId = null,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.Relevance,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var filter = new ProductFilterDto(
            CategoryId: categoryId,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasDiscount: hasDiscount,
            ShopId: shopId,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize
        );

        GetProductsQuery query = new(filter);
        Result<PaginationResult<ProductListItemDto>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.ToResponse("Lấy danh sách sản phẩm thất bại"));
        }

        return Ok(result.ToResponse("Lấy danh sách sản phẩm thành công"));
    }

    /// <summary>
    /// Create a new product with variants and options
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created product</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateProductAsync(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        CreateProductCommand command = new(
            ShopId: request.ShopId,
            CategoryId: request.CategoryId,
            Name: request.Name,
            Description: request.Description,
            Media: request.Media,
            Options: request.Options,
            Variants: request.Variants
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.ToResponse("Tạo sản phẩm thất bại"));
        }

        return Ok(result.ToResponse("Tạo sản phẩm thành công"));
    }

    /// <summary>
    /// Get product details by ID (Public view for customers)
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details for customer view</returns>
    [HttpGet("{productId:guid}/public")]
    public async Task<IActionResult> GetProductPublicAsync(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken = default)
    {
        GetProductQuery query = new(productId);
        Result<ProductDetailDto> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.ToResponse("Lấy thông tin sản phẩm thất bại"));
        }

        return Ok(result.ToResponse("Lấy thông tin sản phẩm thành công"));
    }
}

