using ECommerceBackend.Common.Domain;
using ECommerceBackend.Modules.Users.Application.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerceBackend.Modules.Users.Infrastructure.Identity;

/// HoangDucHiep - 08/2025
/// <summary>
/// Implementation of the identity provider service using Keycloak.
/// This service handles user registration and other identity-related operations.
/// </summary>
/// <remarks>
/// This implementation interacts with Keycloak's Admin REST API to manage users.
/// </remarks>
internal sealed class IdentityProviderService : IIdentityProviderService
{
    private const string PasswordCredentialType = "Password";

    private readonly KeyCloakClient _keyCloakClient;
    private readonly ILogger<IdentityProviderService> _logger;

    public IdentityProviderService(KeyCloakClient keyCloakClient, ILogger<IdentityProviderService> logger)
    {
        _keyCloakClient = keyCloakClient;
        _logger = logger;
    }


    // POST /admin/realms/{realm}/users
    /// HoangDucHiep - 08/2025
    /// <summary>
    /// Registers a new user asynchronously to the identity provider (Keycloak).
    /// This method creates a new user with the provided username and password.
    /// If a user with the same username already exists, it returns a failure result.
    /// </summary>
    /// <param name="user">The user model containing the user's information.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a Result indicating success or failure. On success, it contains the user ID of the newly registered user.
    /// </returns>
    public async Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentation(
            Username: user.Username,
            Email: user.Username,
            Enabled: true,
            EmailVerified: false,
            Credentials:
            [
                new CredentialRepresentation(
                    PasswordCredentialType,
                    user.Password,
                    false)
            ]);

        try
        {
            string identityId = await _keyCloakClient.RegisterUserAsync(userRepresentation, cancellationToken);

            return identityId;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogError(exception, "User registration failed: User with phone {Username} already exists.", user.Username);

            return Result.Failure<string>(IdentityProviderErrors.PhoneIsNotUnique);
        }
    }
}
