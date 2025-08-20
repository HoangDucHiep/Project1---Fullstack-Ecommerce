using Microsoft.AspNetCore.Routing;

namespace ECommerceBackend.Common.Presentation.Endpoints;

/// <summary>
/// Contract for minimal API endpoint modules that can map their routes onto a route builder.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint's routes and handlers onto the provided route builder.
    /// </summary>
    /// <param name="app">The target route builder.</param>
    void MapEndpoints(IEndpointRouteBuilder app);
}
