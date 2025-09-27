using ECommerceBackend.Application.Shops.ShopRegister;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Shops;


[ApiController]
[Route("api/shops")]
public class ShopController : ControllerBase
{
    private readonly ISender _sender;

    public ShopController(ISender sender)
    {
        _sender = sender;
    }


    [HttpPost("resgister")]
    public async Task<IActionResult> Register(
        ShopRegiesterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ShopRegisterCommand(
            request.Name,
            request.Description,
            request.LogoUrl,
            request.BannerUrl,
            request.OwnerId);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
