using System.Security.Cryptography;
using System.Text;
using ECommerceBackend.Application.Abstracts.Encryption;
using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.Extensions.Configuration;

namespace ECommerceBackend.Infrastructure.Encryption;

/// HDHiep - 10/11/2025
/// <summary>
/// Service for encryption, decryption, and hashing
/// </summary>
internal sealed class EncryptionService : IEncryptionService
{

    private readonly string _encryptionKey;
    private readonly byte[] _encryptionKeyBytes;
    private const string CONFIGURATION_SECTION = "Encryption:Key";

    public EncryptionService(IConfiguration configuration)
    {
        _encryptionKey = configuration[CONFIGURATION_SECTION] ?? throw new ApplicationInvalidOperationException(Error.Failure("encryption-service:not-config", "Encryption key is not configured."));

        if (_encryptionKey.Length < 32)
        {
            throw new ApplicationInvalidOperationException(Error.Failure("encryption-service:key-too-short", "Encryption key must be at least 32 characters long."));
        }

        // Use only the first 32 bytes for AES-256

        _encryptionKeyBytes = Encoding.UTF8.GetBytes(_encryptionKey[..32]);
    }

    /// <summary>
    /// Decrypts data that was encrypted using EncryptData
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public Result<string> Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return Result.Failure<string>(Error.Validation("encryption-service:invalid-ciphertext", "Cipher text cannot be null or empty."));
        }

        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _encryptionKeyBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Extract IV from the beginning of the cipher text
            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            string plainText = sr.ReadToEnd();
            return Result.Success(plainText);
        }
        catch (FormatException)
        {
            return Result.Failure<string>(EncryptionErrors.InvalidEncryptedData);
        }
        catch (CryptographicException)
        {
            return Result.Failure<string>(EncryptionErrors.DecryptionFailed);
        }
        catch (Exception)
        {
            return Result.Failure<string>(EncryptionErrors.DecryptionFailed);
        }
    }

    public Result<string> Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return Result.Failure<string>(EncryptionErrors.InvalidEncryptedData);
        }

        try
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKeyBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Generate a new IV for each encryption
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);

            swEncrypt.Write(plainText);
            swEncrypt.Close();

            byte[] encrypted = msEncrypt.ToArray();

            // Combine IV and encrypted data
            byte[] result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

            return Result.Success(Convert.ToBase64String(result));
        }
        catch (Exception)
        {
            return Result.Failure<string>(EncryptionErrors.EncryptionFailed);
        }
    }

    public string GenerateSalt(int size = 16)
    {
        if (size <= 0)
        {
            throw new ApplicationArgumentException(Error.BadRequest("encryption-service:invalid-salt-size", "Salt size must be greater than zero."));
        }

        try
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[size];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            throw new ApplicationInvalidOperationException(Error.Failure("encryption-service:salt-generation-failed", "Failed to generate salt."), ex);
        }
    }

    public string GenerateSecureToken(int length = 32)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than 0.", nameof(length));
        }

        try
        {
            // Calculate required bytes (Base64 encoding uses 4 characters for every 3 bytes)
            int byteLength = (int)Math.Ceiling(length * 3.0 / 4.0);

            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[byteLength];
            rng.GetBytes(bytes);

            string token = Convert.ToBase64String(bytes)
                .Replace('+', '-')  // Make URL-safe
                .Replace('/', '_')  // Make URL-safe
                .TrimEnd('=');      // Remove padding

            // Trim to exact length requested
            return token.Length > length ? token[..length] : token;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate secure token.", ex);
        }
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        try
        {
            // Use BCrypt with work factor of 12 (secure but not too slow)
            return BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt(12));
        }
        catch (Exception ex)
        {
            throw new ApplicationInvalidOperationException(EncryptionErrors.PasswordHashFailed, ex);
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception)
        {
            // Log exception if needed, but return false for security
            return false;
        }
    }
}
