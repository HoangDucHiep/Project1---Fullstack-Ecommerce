namespace ECommerceBackend.Modules.Users.Infrastructure.Identity;

/// HoangDucHiep - 08/2025
internal sealed record CredentialRepresentation(
    string Type,
    string Value,
    bool Temporary);
