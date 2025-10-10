namespace ECommerceBackend.Api.Contracts.Authentication;

/// HDHiep - 10/10/2025
/// <summary>
/// Request model for user login
/// </summary>
/// <param name="Identifier">Email or phone number</param>
/// <param name="Password">User password</param>
public record LoginUserRequest(
    string Identifier,
    string Password
);
