namespace ECommerceBackend.Api.Contracts.Authentication;


/// HDHiep - 10/06/2025
/// <summary>
/// Request model for registering a new user
/// </summary>
/// <param name="PhoneNumber"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record RegisterUserRequest(
    string PhoneNumber,
    string? Password = null
);
