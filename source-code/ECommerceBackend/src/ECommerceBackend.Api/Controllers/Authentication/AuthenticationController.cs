using ECommerceBackend.Api.Contracts.Authentication;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Authentication;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _sender;

    public AuthenticationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(request.PhoneNumber, request.Password);
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new AuthenticationResponse
        {
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken,
            AccessTokenExpiration = result.Value.AccessTokenExpiration,
            RefreshTokenExpiration = result.Value.RefreshTokenExpiration,
            IdentityUserId = result.Value.IdentityUserId
        });
    }
}
