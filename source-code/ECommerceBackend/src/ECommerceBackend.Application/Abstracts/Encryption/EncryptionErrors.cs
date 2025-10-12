using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Encryption;

/// HDHiep - 10/11/2025
/// <summary>
/// Common encryption-related errors
/// </summary>
public static class EncryptionErrors
{
    public static readonly Error InvalidPassword = Error.Validation(
        "Encryption.InvalidPassword",
        "The provided password is invalid.");

    public static readonly Error PasswordHashFailed = Error.Failure(
        "Encryption.PasswordHashFailed",
        "Failed to hash the password.");

    public static readonly Error EncryptionFailed = Error.Failure(
        "Encryption.EncryptionFailed",
        "Failed to encrypt the data.");

    public static readonly Error DecryptionFailed = Error.Failure(
        "Encryption.DecryptionFailed",
        "Failed to decrypt the data.");

    public static readonly Error InvalidEncryptedData = Error.BadRequest(
        "Encryption.InvalidEncryptedData",
        "The encrypted data format is invalid.");

    public static readonly Error TokenGenerationFailed = Error.Failure(
        "Encryption.TokenGenerationFailed",
        "Failed to generate secure token.");
}
