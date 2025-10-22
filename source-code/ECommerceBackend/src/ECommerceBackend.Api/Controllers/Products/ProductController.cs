using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Products.Commands.CreateProduct;
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
}

