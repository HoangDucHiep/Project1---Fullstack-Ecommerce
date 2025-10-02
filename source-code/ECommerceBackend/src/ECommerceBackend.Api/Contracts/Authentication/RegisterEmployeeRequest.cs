namespace ECommerceBackend.Api.Contracts.Authentication;

public record RegisterEmployeeRequest(
    string Email,
    string Password
);
