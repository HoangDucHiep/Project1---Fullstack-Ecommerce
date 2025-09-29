using ECommerceBackend.Application.Categories.DeleteCategory;
using ECommerceBackend.Application.Categories.GetCategoryById;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Categories;


[ApiController]
[Route("api/v1/categories")]
public class CategoryController : ControllerBase
{
    // Implement category-related endpoints here
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }
    [HttpDelete("{CategoryId:guid}")]
    public async Task<IActionResult> Delete(Guid CategoryId, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(CategoryId);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpGet("categories/{CategoryId:guid}")]
    public async Task<IActionResult> GetById(Guid CategoryId, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(CategoryId);
        Result<CategoryResponse> result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }



}

