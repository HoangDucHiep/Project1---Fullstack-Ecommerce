using ECommerceBackend.Api.Contracts.Addresses;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Addresses.AddNewAddress;
using ECommerceBackend.Application.Addresses.DeleteAddress;
using ECommerceBackend.Application.Addresses.GetAddressById;
using ECommerceBackend.Application.Addresses.GetAddressesOfCurrentUser;
using ECommerceBackend.Application.Addresses.UpdateAddress;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;
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

    //PBNMinh- 10/10/2025
    [HttpPost("/me/addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressCreateRequest request)
    {
        if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
        { return Unauthorized(); }

        var userId = Guid.Parse(_userContext.UserId!);

        var command = new AddNewAddressCommand(
            request.Name,
            request.Phone,
            request.Province,
            request.District,
            request.Ward,
            request.AddressLine,
            request.IsDefault,
            request.IsPickUpAddress,
            request.IsReturnAddress,
            userId 
        );

        Result<AddressDto> result = await _sender.Send(command);

        if (result.IsFailure)
        { return StatusCode(result.Error.Type.StatusCode, result.Error); }

        return CreatedAtAction(nameof(GetAddressById), new { addressId = result.Value.Id }, result.Value);
    }




    [HttpGet("/me/addresses/{addressId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAddressById([FromRoute] Guid addressId)
    {
        Console.WriteLine($"Is Authenticated: {_userContext.IsAuthenticated}");
        Console.WriteLine($"UserId: {_userContext.UserId}");

        var query = new GetAddressByIdQuery(addressId);

        Result<AddressDto> result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.Value);
    }

    //PBNMinh- 11/10/2025
    [HttpGet("/me/addresses")]
    public async Task<IActionResult> GetAddressesOfCurrentUser([FromQuery] GetAddressOfCurrentUserRequest request)
    {
        if (!_userContext.IsAuthenticated)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var query = new GetAddressesOfCurrentUserQuery(
            request.Page,
            request.PageSize
        );

        Result<PaginationResult<AddressDto>> result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.Value);
    }



    [HttpPut("/me/addresses/{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress([FromRoute] Guid addressId, [FromBody] AddressUpdateRequest request)
    {
        var command = new UpdateAddressCommand(addressId, request.Name, request.Phone, request.Province, request.District, request.Ward, request.AddressLine, request.IsDefault, request.IsPickUpAddress, request.IsReturnAddress);
        Result<AddressDto> result = await _sender.Send(command);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }
        return Ok(result.Value);
    }

    //PBNMinh- 10/10/2025
    [HttpDelete("/me/addresses/{addressId:guid}")]
    public async Task<IActionResult> DeleteAddress([FromRoute] Guid addressId)
    {
        if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
        { return Unauthorized(); }

        var userId = Guid.Parse(_userContext.UserId!);

        var command = new DeleteAddressCommand(addressId, userId);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        { return StatusCode(result.Error.Type.StatusCode, result.Error); }

        return NoContent();
    }

}
