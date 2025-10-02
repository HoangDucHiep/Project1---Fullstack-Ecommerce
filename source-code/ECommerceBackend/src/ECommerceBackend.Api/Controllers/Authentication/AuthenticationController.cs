using ECommerceBackend.Api.Contracts.Authentication;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Authentication.Login;
using ECommerceBackend.Application.Authentication.Logout;
using ECommerceBackend.Application.Authentication.RefreshToken;
using ECommerceBackend.Application.Authentication.Register;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        var command = new RegisterUserCommand(request.PhoneNumber, request.Email, request.Password);
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new
        {
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.ExpiresAt,
            result.Value.UserId
        });
    }

    [HttpPost("register/employee")]
    public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeRequest request)
    {
        var command = new RegisterEmployeeCommand(request.Email, request.Password);
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new
        {
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.ExpiresAt,
            result.Value.UserId
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Identifier, request.Password);
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new
        {
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.ExpiresAt,
            result.Value.UserId
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new
        {
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.ExpiresAt,
            result.Value.UserId
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutCommand();
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { Message = "Logged out successfully" });
    }
}
