using ECommerceBackend.Common.Domain;

namespace ECommerceBackend.Modules.Users.Application.Identity;

/// <summary>
/// Service for user identity management, such as registration
/// </summary>
public interface IIdentityProviderService
{
    /// <summary>
    /// Registers a new user asynchronously to the identity provider.
    /// </summary>
    /// <param name="user">The user model containing registration details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// a Result indicating success or failure, and on success, the user ID of the newly registered user.</returns>
    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
}

