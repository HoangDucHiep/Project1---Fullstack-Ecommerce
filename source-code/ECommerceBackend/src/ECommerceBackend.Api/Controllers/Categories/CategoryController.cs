using System.Xml.Linq;
using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Api.Controllers.Users;
using ECommerceBackend.Application.Users.RegisterUser;
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

    [HttpPost("create")]
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
}
