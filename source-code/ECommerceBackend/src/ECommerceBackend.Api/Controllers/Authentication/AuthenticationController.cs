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

    [HttpPost("register/phone")]
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


    [HttpPost("register/phone/verify")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> VerifyPhoneNumber()
    {
        throw new NotImplementedException();
    }


    [HttpPost("register/phone/resend")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ResendPhoneNumberVerification()
    {
        throw new NotImplementedException();
    }

    [HttpPost("oauth/google")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> GoogleOAuth()
    {
        throw new NotImplementedException();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var command = new LoginUserCommand(request.Identifier, request.Password);
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

    [HttpPost("login/phone/verify")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> VerifyLoginWithPhoneNumber()
    {
        throw new NotImplementedException();
    }

    // refresh token
    [HttpPost("refresh")]
    public Task<IActionResult> RefreshToken()
    {
        throw new NotImplementedException();
    }

    // logout
    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        throw new NotImplementedException();
    }

    // forgot-password
    [HttpPost("forgot-password")]
    public Task<IActionResult> ForgotPassword()
    {
        throw new NotImplementedException();
    }


}
