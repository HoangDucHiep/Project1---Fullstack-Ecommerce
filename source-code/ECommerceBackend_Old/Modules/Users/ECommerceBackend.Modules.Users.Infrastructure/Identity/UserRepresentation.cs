namespace ECommerceBackend.Modules.Users.Infrastructure.Identity;

internal sealed record UserRepresentation(
    string Username,
    string Email,
    bool Enabled,
    bool EmailVerified,
    CredentialRepresentation[] Credentials);
