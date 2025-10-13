using ECommerceBackend.Api.Contracts.Authentication;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Authentication;
using ECommerceBackend.Application.Authentication.LoginUserWithOtp;
using ECommerceBackend.Application.Authentication.Register;
using ECommerceBackend.Application.Authentication.RegisterUserWithOtp;
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

    #region Registration Endpoints

    /// <summary>
    /// Instance Registration for quick development and testing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
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

    [HttpPost("register/phone/otp")]
    public async Task<IActionResult> RegisterUserWithOtp([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterWithOtpCommand(request.PhoneNumber, request.Password);
        Result result = await _sender.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { Message = "OTP sent successfully" });
    }

    [HttpPost("register/phone/otp/verify")]
    public async Task<IActionResult> VerifyPhoneNumber(VerifyOtpRegisterRequest request)
    {
        var command = new VerifyRegistrationOtpCommand(request.phoneNumber, request.otp);

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

    [HttpPost("register/phone/otp/resend")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendPhoneNumberVerification(RegisterUserCommandRequest request)
    {
        var command = new RegisterOtpResendCommand(request.PhoneNumber);

        Result<int> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { Message = "OTP resent successfully", ResendLeft = result.Value });
    }

    #endregion

    #region Login Endpoints

    /// <summary>
    /// Instance Login for quick development and testing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
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

    [HttpPost("login/otp")]
    public async Task<IActionResult> LoginWithOtp([FromBody] LoginUserRequest request)
    {
        var command = new LoginWithOtpCommand(request.Identifier, request.Password);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { Message = "OTP sent successfully for login verification" });
    }

    [HttpPost("login/otp/verify")]
    public async Task<IActionResult> VerifyLoginOtp([FromBody] VerifyLoginOtpRequest request)
    {
        var command = new VerifyLoginOtpCommand(request.Identifier, request.Otp);
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

    [HttpPost("login/otp/resend")]
    public async Task<IActionResult> ResendLoginOtp([FromBody] LoginOtpResendRequest request)
    {
        var command = new LoginOtpResendCommand(request.Identifier);
        Result<int> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { Message = "Login OTP resent successfully", ResendLeft = result.Value });
    }

    #endregion
}
