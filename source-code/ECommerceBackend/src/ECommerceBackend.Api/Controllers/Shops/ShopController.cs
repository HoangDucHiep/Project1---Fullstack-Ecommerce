using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Contracts.Shops;
using ECommerceBackend.Application.Shops.GetShopByUserId;
using ECommerceBackend.Application.Shops.RegisterNewShop;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Shops;

[ApiController]
[Route("api/v1/shops")]
public class ShopController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;

    public ShopController(ISender sender, IUserContext userContext)
    {
        _sender = sender;
        _userContext = userContext;
    }

    /// <summary>
    /// Register a new shop for the current authenticated user
    /// </summary>
    /// <param name="request">Shop registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created shop ID</returns>
    [HttpPost]
    public async Task<IActionResult> RegisterShopAsync(
        [FromBody] RegisterNewShopRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check if user is authenticated
        if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "User is not authenticated"
            });
        }

        var ownerId = Guid.Parse(_userContext.UserId);

        var command = new RegisterNewShopCommand(
            request.Name,
            request.Description,
            request.LogoUrl,
            request.BannerUrl,
            ownerId
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        object response = result.ToResponse("Đăng ký shop thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    /// <summary>
    /// Get shop information by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Shop information or null if user doesn't have a shop</returns>
    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetShopByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetShopByUserIdQuery(userId);

        Result<ShopDto?> result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            object errorResponse = result.ToResponse("Lỗi khi lấy thông tin shop");
            return StatusCode(result.Error.GetStatusCode(), errorResponse);
        }

        if (result.Value == null)
        {
            // User chưa có shop
            return Ok(new
            {
                success = true,
                message = "Người dùng chưa có shop",
                data = (object?)null,
                hasShop = false
            });
        }

        // User có shop
        return Ok(new
        {
            success = true,
            message = "Lấy thông tin shop thành công",
            data = result.Value,
            hasShop = true
        });
    }

    /// <summary>
    /// Get shop information of the current authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Shop information or null if user doesn't have a shop</returns>
    [HttpGet("my-shop")]
    public async Task<IActionResult> GetMyShopAsync(CancellationToken cancellationToken = default)
    {
        // Check if user is authenticated
        if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "User is not authenticated"
            });
        }

        var userId = Guid.Parse(_userContext.UserId);
        var query = new GetShopByUserIdQuery(userId);

        Result<ShopDto?> result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            object errorResponse = result.ToResponse("Lỗi khi lấy thông tin shop");
            return StatusCode(result.Error.GetStatusCode(), errorResponse);
        }

        if (result.Value == null)
        {
            // User chưa có shop
            return Ok(new
            {
                success = true,
                message = "Bạn chưa có shop",
                data = (object?)null,
                hasShop = false
            });
        }

        // User có shop
        return Ok(new
        {
            success = true,
            message = "Lấy thông tin shop thành công",
            data = result.Value,
            hasShop = true
        });
    }
}

