using ECommerceBackend.Application.Categories;
using ECommerceBackend.Application.Categories.GetCategories;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;
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


    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        Result<List<CategoriesDTO>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }


}
