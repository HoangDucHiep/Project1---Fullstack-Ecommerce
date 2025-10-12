using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Encryption;


/// HDHiep - 10/11/2025
/// <summary>
/// Service for encryption, decryption, and hashing
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Hashes a password using a secure algorithm (BCrypt)
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <returns>The hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <param name="hashedPassword">The hashed password</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Encrypts sensitive data using AES encryption
    /// </summary>
    /// <param name="plainText">The data to encrypt</param>
    /// <returns>Result containing encrypted data or error</returns>
    Result<string> Encrypt(string plainText);

    /// <summary>
    /// Decrypts data that was encrypted using EncryptData
    /// </summary>
    /// <param name="cipherText">The encrypted data</param>
    /// <returns>Result containing decrypted data or error</returns>
    Result<string> Decrypt(string cipherText);

    /// <summary>
    /// Generates a cryptographically secure random token
    /// </summary>
    /// <param name="length">The length of the token (default: 32)</param>
    /// <returns>A secure random token</returns>
    string GenerateSecureToken(int length = 32);

    /// <summary>
    /// Generates a secure salt for additional encryption operations
    /// </summary>
    /// <param name="size">The size of the salt (default: 16)</param>
    /// <returns>A cryptographically secure salt</returns>
    string GenerateSalt(int size = 16);
}
