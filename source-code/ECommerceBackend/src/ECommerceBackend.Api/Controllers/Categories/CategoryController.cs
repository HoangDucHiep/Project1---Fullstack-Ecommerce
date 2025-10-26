using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Api.Extensions;
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

    [HttpGet("Get")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        Result<List<CategoriesDTO>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.ToResponse("Lấy danh sách danh mục thành công"));
    }


}
