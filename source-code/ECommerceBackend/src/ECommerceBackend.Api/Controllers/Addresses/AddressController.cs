using ECommerceBackend.Api.Contracts.Addresses;
using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Addresses.AddNewAddress;
using ECommerceBackend.Application.Addresses.DeleteAddress;
using ECommerceBackend.Application.Addresses.GetAddressById;
using ECommerceBackend.Application.Addresses.GetAddressesOfCurrentUser;
using ECommerceBackend.Application.Addresses.UpdateAddress;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Addresses;

///  HDHiep - 10/01/2025
/// <summary>
/// Controller for managing user addresses.
/// </summary>
[ApiController]
[Route("api/v1")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;

    public AddressController(ISender sender, IUserContext userContext)
    {
        _sender = sender;
        _userContext = userContext;
    }


    [HttpPost("/me/addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressCreateRequest request)
    {
        var command = new AddNewAddressCommand(request.Name, request.Phone, request.Province, request.District, request.Ward, request.AddressLine, request.IsDefault, request.IsPickUpAddress, request.IsReturnAddress);
        Result<AddressDto> result = await _sender.Send(command);

        object response = result.ToResponse("Tạo địa chỉ thành công");
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAddressById), new { addressId = result.Value.Id }, response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    [HttpGet("/me/addresses/{addressId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAddressById([FromRoute] Guid addressId)
    {
        Console.WriteLine($"Is Authenticated: {_userContext.IsAuthenticated}");
        Console.WriteLine($"UserId: {_userContext.UserId}");

        Result<AddressDto> result = await _sender.Send(new GetAddressByIdQuery(addressId));
        return Ok(result.ToResponse("Lấy thông tin địa chỉ thành công"));
    }

    [HttpGet("/me/addresses")]
    public async Task<IActionResult> GetAddressesOfCurrentUser([FromQuery] GetAddressOfCurrentUserRequest request)
    {
        Console.WriteLine($"Is Authenticated: {_userContext.IsAuthenticated}");
        Console.WriteLine($"UserId: {_userContext.UserId}");

        Result<PaginationResult<AddressDto>> result = await _sender.Send(new GetAddressesOfCurrentUserQuery(request.Page, request.PageSize));
        return Ok(result.ToPaginatedResponse("Lấy danh sách địa chỉ thành công"));
    }

    [HttpPut("/me/addresses/{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress([FromRoute] Guid addressId, [FromBody] AddressUpdateRequest request)
    {
        var command = new UpdateAddressCommand(addressId, request.Name, request.Phone, request.Province, request.District, request.Ward, request.AddressLine, request.IsDefault, request.IsPickUpAddress, request.IsReturnAddress);
        Result<AddressDto> result = await _sender.Send(command);

        return Ok(result.ToResponse("Cập nhật địa chỉ thành công"));
    }

    [HttpDelete("/me/addresses/{addressId:guid}")]
    public async Task<IActionResult> DeleteAddress([FromRoute] Guid addressId)
    {
        var command = new DeleteAddressCommand(addressId);
        Result result = await _sender.Send(command);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }
        return NoContent();
    }
}
