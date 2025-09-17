using System.Net.Http.Json;

namespace ECommerceBackend.Modules.Users.Infrastructure.Identity;

/// HoangDucHiep - 08/2025
/// <summary>
/// Client for interacting with Keycloak identity provider.
/// </summary>
/// <remarks>
/// This client is responsible for making HTTP requests to Keycloak's REST API.
/// It handles user registration and other identity-related operations.
/// </remarks> 
internal sealed class KeyCloakClient
{
    private readonly HttpClient _httpClient;

    public KeyCloakClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// HoangDucHiep - 08/2025
    /// <summary>
    /// Registers a new user in Keycloak.
    /// </summary>
    /// <param name="user">The user representation containing user details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The ID of the newly registered user.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response is invalid or missing required information.</exception>
    /// <remarks>
    /// This method sends a POST request to the Keycloak server to create a new user.
    /// On success, it extracts the user ID from the Location header of the response.
    /// </remarks>
    internal async Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync(
            requestUri: "users",
            value: user,
            cancellationToken: cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return ExtractIdentityIdFromLocationHeader(httpResponseMessage);
    }


    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage httpResponseMessage)
    {
        const string userSegmentName = "users";

        string? locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header is missing in the response.");
        }

        int userSegmentValueIndex = locationHeader.IndexOf(userSegmentName, StringComparison.InvariantCultureIgnoreCase) + 1;

        string identityId = locationHeader[(userSegmentValueIndex + userSegmentName.Length)..];

        return identityId;
    }
}
