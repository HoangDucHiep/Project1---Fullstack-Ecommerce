namespace ECommerceBackend.Application.Contracts.Commons;

/// <summary>
/// Represents a response containing a list of links.
/// Support for HATEOAS (Hypermedia as the Engine of Application State).
/// Each link provides information about related resources or actions.
/// The Links property is a collection of LinkDto objects, each representing a single link with its associated metadata.
/// </summary>
/// <remarks>
/// This interface is useful for implementing HATEOAS principles in RESTful APIs,
/// allowing clients to navigate the API dynamically based on the links provided in the response.
/// </remarks>
/// <typeparam name="LinkDto">The type of the link data transfer object.</typeparam>
public interface ILinksResponse
{
    List<LinkDto> Links { get; set; }
}
