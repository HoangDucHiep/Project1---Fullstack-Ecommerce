using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Categories;
using ECommerceBackend.Application.Categories.GetCategories;
using ECommerceBackend.Application.Categories.GetCategotyByID;
using ECommerceBackend.Application.Categories.SearchCategory;
using ECommerceBackend.Application.Categories.UpdateCategory;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;
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

    /// PBNMinh
    [HttpPost("create")]
    public async Task<IActionResult> CreateCategoryAsync(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        CreateCategoryCommand command = new(
            request.Name,
            request.IconUrl,
            request.ParentId
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.ToResponse("Thêm mới danh mục thành công"));
    }


    /// PBNMinh
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery(Name = "NameOfCategory")] string queryText, CancellationToken cancellationToken)
    {
        var query = new SearchCategoryQuery(queryText);
        Result<List<CategoryTreeDto>> result = await _sender.Send(query, cancellationToken);

        return Ok(result.ToResponse("Tìm kiếm danh mục thành công"));
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        Result<List<GetCategoriesTreeDTO>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.ToResponse("Lấy danh sách danh mục thành công"));
    }

    /// PBNMinh
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(
    [FromRoute] string id,
    [FromQuery] bool includeChildren = false,
    CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out Guid categoryId))
        {
            //return NotFound(new { message = $"Category with ID '{id}' was not found." });
             return StatusCode( CategoryErrors.NotFound(id).Type.StatusCode, CategoryErrors.NotFound(id)
    );
        }

        var query = new GetCategoryByIdQuery(categoryId, includeChildren);
        Result<object> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.Value);
    }


    /// PBNMinh
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategoryAsync(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        UpdateCategoryCommand command = new(
            id,
            request.Name,
            request.IconUrl,
            request.Status,
            request.NewParentId
        );

        Result<CategoryDto> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.Value);
    }


}
