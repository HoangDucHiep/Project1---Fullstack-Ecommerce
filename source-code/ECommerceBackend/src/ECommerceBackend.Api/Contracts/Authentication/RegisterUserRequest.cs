namespace ECommerceBackend.Api.Contracts.Authentication;

public record RegisterUserRequest(
    string PhoneNumber,
    string? Email = null,
    string? Password = null
);
