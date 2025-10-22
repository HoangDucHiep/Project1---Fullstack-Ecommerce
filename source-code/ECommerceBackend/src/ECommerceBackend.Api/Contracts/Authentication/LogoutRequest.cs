namespace ECommerceBackend.Api.Contracts.Authentication;

/// <summary>
/// Request model for user logout
/// </summary>
/// <param name="UserId">The user ID to logout</param>
public record LogoutRequest(
    string UserId
);
