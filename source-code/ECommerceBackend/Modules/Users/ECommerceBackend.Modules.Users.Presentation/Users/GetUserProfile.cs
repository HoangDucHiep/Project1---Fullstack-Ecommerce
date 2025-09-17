using ECommerceBackend.Common.Domain;
using ECommerceBackend.Common.Presentation.Endpoints;
using ECommerceBackend.Common.Presentation.Results;
using ECommerceBackend.Modules.Users.Application.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerceBackend.Modules.Users.Presentation.Users;
internal sealed class GetUserProfile : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{id}/profile", async (Guid id, ISender sender) =>
        {
            Result<UserResponse> result = await sender.Send(new GetUserQuery(id));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
