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


}
