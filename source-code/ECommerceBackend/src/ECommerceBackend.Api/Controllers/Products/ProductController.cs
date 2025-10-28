using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Commands.CreateNewProduct;
using ECommerceBackend.Application.Products.Queries.GetProductDetails;
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
}
