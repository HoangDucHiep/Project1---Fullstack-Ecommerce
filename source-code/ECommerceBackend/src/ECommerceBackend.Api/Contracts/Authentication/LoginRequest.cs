namespace ECommerceBackend.Api.Contracts.Authentication;
public record LoginRequest(
    string Identifier, // Email hoặc Phone
    string Password
);
