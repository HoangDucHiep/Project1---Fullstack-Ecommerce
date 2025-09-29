using ECommerceBackend.Application.Users.GetUserById;
using ECommerceBackend.Application.Users.RegisterUser;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Users;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }



    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        UserRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(userId);
        Result<CategoryResponse> result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

}
