using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Commands.CreateNewProduct;
using ECommerceBackend.Application.Products.Queries;
using ECommerceBackend.Application.Products.Queries.GetProductBySlug;
using ECommerceBackend.Application.Products.Queries.GetProductDetails;
using ECommerceBackend.Application.Products.Queries.GetProductsByCategory;
using ECommerceBackend.Application.Products.Queries.GetProductsByShop;
using ECommerceBackend.Application.Products.Queries.SearchProducts;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Products;

[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a new product with variants and media
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product details</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(
    [FromBody] CreateNewProductRequest request,
    CancellationToken cancellationToken = default)
    {
        var command = new CreateNewProductCommand(
            ShopId: request.ShopId,
            CategoryId: request.CategoryId,
            Name: request.Name,
            Description: request.Description,
            Sku: request.Sku, // Product SKU
            Medias: request.Medias.Select(m => new CreateProductMediaDto(
                MediaName: m.MediaName,
                MediaUrl: m.MediaUrl,
                IsCover: m.IsCover,
                SortOrder: m.SortOrder)).ToList(),
            Options: request.Options.Select(o => new CreateProductOptionDto(o.Name, o.Values)).ToList(),
            Variants: request.Variants.Select(v => new CreateProductVariantDto(
                OptionValues: v.OptionValues,
                Price: v.Price,
                Stock: v.Stock,
                Sku: v.Sku, // Variant SKU
                Weight: v.Weight,
                Height: v.Height,
                Width: v.Width,
                Length: v.Length,
                Medias: v.Medias?.Select(m => new CreateProductMediaDto(
                    MediaName: m.MediaName,
                    MediaUrl: m.MediaUrl,
                    IsCover: m.IsCover,
                    SortOrder: m.SortOrder)).ToList()
            )).ToList(),
            DefaultPrice: request.DefaultPrice,
            DefaultStock: request.DefaultStock,
            DefaultWeight: (double?)request.DefaultWeight,
            DefaultHeight: (double?)request.DefaultHeight,
            DefaultWidth: (double?)request.DefaultWidth,
            DefaultLength: (double?)request.DefaultLength
        );

        Result<ProductDetailDto> result = await _sender.Send(command, cancellationToken);

        object response = result.ToResponse("Tạo sản phẩm thành công");

        return result.IsSuccess
            ? Created($"/api/v1/products/{result.Value.Id}", response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

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
        var query = new SearchProductsQuery(
            Q: q,
            CategoryId: categoryId,
            ShopId: shopId,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasPromotion: hasPromotion,
            PickupProvince: pickupProvince,
            PickupDistrict: pickupDistrict,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize
        );

        Result<PaginationResult<ProductDto>> result = await _sender.Send(query, cancellationToken);

        object response = result.ToPaginatedResponse<ProductDto>("Tìm kiếm sản phẩm thành công");

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
        var query = new GetProductsByCategoryQuery(
            CategoryId: categoryId,
            Q: q,
            ShopId: shopId,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasPromotion: hasPromotion,
            PickupProvince: pickupProvince,
            PickupDistrict: pickupDistrict,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize
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
        var query = new GetProductsByShopQuery(
            ShopId: shopId,
            Q: q,
            CategoryId: categoryId,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            HasPromotion: hasPromotion,
            PickupProvince: pickupProvince,
            PickupDistrict: pickupDistrict,
            SortBy: sortBy,
            Page: page,
            PageSize: pageSize
        );

        Result<PaginationResult<ProductDto>> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy danh sách sản phẩm theo shop thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }
}
