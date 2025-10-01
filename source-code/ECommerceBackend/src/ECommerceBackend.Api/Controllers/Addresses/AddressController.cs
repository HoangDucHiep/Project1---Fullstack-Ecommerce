using ECommerceBackend.Api.Contracts.Addresses;
using ECommerceBackend.Application.Addresses.AddNewAddress;
using ECommerceBackend.Application.Addresses.GetAddressById;
using ECommerceBackend.Application.Addresses.GetAddressesOfCurrentUser;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Addresses;

[ApiController]
public class AddressController : ControllerBase
{
    private readonly ISender _sender;

    public AddressController(ISender sender)
    {
        _sender = sender;
    }


    [HttpPost("/me/addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressCreateRequest request)
    {
        var command = new AddNewAddressCommand(request.Name, request.Phone, request.Province, request.District, request.Ward, request.AddressLine, request.IsDefault, request.IsPickUpAddress, request.IsReturnAddress);

        Result<AddressDto> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return CreatedAtAction(nameof(GetAddressById), new { addressId = result.Value.Id }, result.Value);
    }

    [HttpGet("/me/addresses/{addressId:guid}")]
    public async Task<IActionResult> GetAddressById([FromRoute] Guid addressId)
    {
        var query = new GetAddressByIdQuery(addressId);

        Result<AddressDto> result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.Type.StatusCode, result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("/me/addresses")]
    public async Task<IActionResult> GetAddressesOfCurrentUser([FromQuery] GetAddressOfCurrentUserRequest request)
    {
        var query = new GetAddressesOfCurrentUserQuery(request.Page, request.PageSize);
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
        // var command = new UpdateAddressCommand(addressId, request.Name, request.Phone, request.Province, request.District, request.Ward, request.AddressLine, request.IsDefault, request.IsPickUpAddress, request.IsReturnAddress);
        // Result<AddressDto> result = await _sender.Send(command);
        // if (result.IsFailure)
        // {
        //     return StatusCode(result.Error.Type.StatusCode, result.Error);
        // }
        // return Ok(result.Value);
        return StatusCode(501); // Not Implemented
    }
