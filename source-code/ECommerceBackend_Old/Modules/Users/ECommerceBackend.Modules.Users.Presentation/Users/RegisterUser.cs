using ECommerceBackend.Common.Domain;
using ECommerceBackend.Common.Presentation.Endpoints;
using ECommerceBackend.Common.Presentation.Results;
using ECommerceBackend.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerceBackend.Modules.Users.Presentation.Users;

/// HoangDucHiep - 08/2025
/// <summary>
/// Endpoint for user registration
/// </summary>
internal sealed class RegisterUser : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (Request request, ISender sender) =>
        {
            Result<Guid> result = await sender.Send(new RegisterUserCommand(
                request.Phone,
                request.Password));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
            .AllowAnonymous()
            .WithTags(Tags.Users);
    }
}

internal sealed class Request
{
    public string Phone { get; init; }

    public string Password { get; init; }
}
