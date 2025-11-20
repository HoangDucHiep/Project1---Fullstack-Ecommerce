using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Commands.CreateNewProduct;
using ECommerceBackend.Application.Products.Commands.UpdateProduct;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Products;


[ApiController]
[Route("api/v1/seller/products")]
[Authorize]
public class SellerProductController : ControllerBase
{
    private readonly ISender _sender;

    public SellerProductController(ISender sender)
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
            Images: request.Images.Select(img => new CreateProductImageDto(
                ImageUrl: img.ImageUrl,
                IsCover: img.IsCover,
                SortOrder: img.SortOrder)).ToList(),
            Video: request.Video != null 
                ? new CreateProductVideoDto(
                    VideoUrl: request.Video.VideoUrl,
                    SortOrder: request.Video.SortOrder)
                : null,
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
                Images: v.Images?.Select(img => new CreateProductImageDto(
                    ImageUrl: img.ImageUrl,
                    IsCover: img.IsCover,
                    SortOrder: img.SortOrder)).ToList()
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

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Product update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product details</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProductAsync(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateProductCommand(
            Id: id,
            CategoryId: request.CategoryId,
            Name: request.Name,
            Description: request.Description,
            Sku: request.Sku,
            Status: request.Status,
            Medias: request.Medias.Select(m => new UpdateProductMediaDto(
                MediaUrl: m.MediaUrl,
                Id: m.Id,
                IsCover: m.IsCover,
                SortOrder: m.SortOrder)).ToList(),
            Options: request.Options.Select(o => new UpdateProductOptionDto(
                Name: o.Name,
                Values: o.Values.Select(v => new UpdateProductOptionValueDto(
                    Value: v.Value,
                    Id: v.Id)).ToList(),
                Id: o.Id)).ToList(),
            Variants: request.Variants.Select(v => new UpdateProductVariantDto(
                OptionValues: v.OptionValues,
                Price: v.Price,
                Stock: v.Stock,
                Id: v.Id,
                Sku: v.Sku,
                Weight: (double?)v.Weight,
                Height: (double?)v.Height,
                Width: (double?)v.Width,
                Length: (double?)v.Length,
                Medias: v.Medias?.Select(m => new UpdateProductMediaDto(
                    MediaUrl: m.MediaUrl,
                    Id: m.Id,
                    IsCover: m.IsCover,
                    SortOrder: m.SortOrder)).ToList())).ToList(),
            DefaultPrice: request.DefaultPrice,
            DefaultStock: request.DefaultStock,
            DefaultWeight: (double?)request.DefaultWeight,
            DefaultHeight: (double?)request.DefaultHeight,
            DefaultWidth: (double?)request.DefaultWidth,
            DefaultLength: (double?)request.DefaultLength
        );

        Result<ProductDetailDto> result = await _sender.Send(command, cancellationToken);

        object response = result.ToResponse("Cập nhật sản phẩm thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

}
