using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Application.Categories;
using ECommerceBackend.Application.Categories.GetCategories;
using ECommerceBackend.Application.Categories.SearchCategory;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Categories;

/// PBNMinh- 08/09/2025
[ApiController]
[Route("api/v1/categories")]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }

    
    [HttpPost("Create")]
    public async Task<IActionResult> Create(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.IconUrl,
            request.ParentId,
            request.Lft,
            request.Rgt,
            request.Depth

            );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// PBNMinh
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery(Name = "NameOfCategory")] string queryText, CancellationToken cancellationToken)
    {
        var query = new SearchCategoryQuery(queryText);
        Result<List<CategoryDto>> result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
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
