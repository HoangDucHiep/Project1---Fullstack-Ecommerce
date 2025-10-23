using ECommerceBackend.Api.Contracts.Authentication;
using ECommerceBackend.Api.Extensions;
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

        object response = result.ToResponse("Đăng ký thành công");
        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    [HttpPost("register/phone/otp")]
    public async Task<IActionResult> RegisterUserWithOtp([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterWithOtpCommand(request.PhoneNumber, request.Password);
        Result result = await _sender.Send(command);

        return Ok(result.ToResponse("Mã OTP đã được gửi thành công"));
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

        object response = result.ToResponse("Đăng nhập thành công");
        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
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

    #region Token Management Endpoints

    /// <summary>
    /// Refresh access token using a valid refresh token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return StatusCode(result.Error.GetStatusCode(), result.ToResponse("Refresh token failed"));
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

    /// <summary>
    /// Logout user and revoke all refresh tokens
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var command = new LogoutCommand(request.UserId);
        Result result = await _sender.Send(command);

        object response = result.ToResponse("Logout successful");
        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    #endregion
}
