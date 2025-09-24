using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public sealed class CategoryController : ControllerBase
{

    [HttpGet]
    public List<string> GetAll()
    {
        var categories = new List<string> { "Electronics", "Clothing", "Books" };
        return categories;
    }
}
