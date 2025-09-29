using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Modules.Users.Infrastructure.Identity;

/// HoangDucHiep - 08/2025
/// <summary>
/// A delegating handler that adds Keycloak authentication to outgoing HTTP requests.
/// </summary>
/// <remarks>
/// This handler retrieves an access token from Keycloak using client credentials
/// and adds it to the Authorization header of each request.
/// </remarks>
internal sealed class KeyCloakAuthDelegatingHandler : DelegatingHandler
{
    private readonly KeyCloakOptions _options;

    public KeyCloakAuthDelegatingHandler(IOptions<KeyCloakOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Sends an HTTP request with Keycloak authentication.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the access token from Keycloak
        AuthToken authorizationToken = await GetAuthorizationToken(cancellationToken);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", authorizationToken.AccessToken);

        // Use access token in the request
        HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return httpResponseMessage;
    }

    /// <summary>
    /// Retrieves an authorization token from Keycloak using client credentials.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    private async Task<AuthToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        var authRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _options.ConfidentialClientId),
            new("client_secret", _options.ConfidentialClientSecret),
            new("grant_type", "client_credentials"),
            new("scope", "openid")
        };

        using var authRequestContent = new FormUrlEncodedContent(authRequestParameters);

        using var authRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_options.TokenUrl));

        authRequest.Content = authRequestContent;

        using HttpResponseMessage authorizationResponse = await base.SendAsync(authRequest, cancellationToken);

        authorizationResponse.EnsureSuccessStatusCode();

        return await authorizationResponse.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize authorization token.");
    }

    internal sealed class AuthToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
    }
}
