using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Application.FileStorage;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Infrastructure.FileStorage;

/// <summary>
/// HDHiep - 10/28/2025
/// Implementation of IFileStorageService that stores files on Amazon S3.
/// </summary>
public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Settings _s3Settings;
    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(
        IAmazonS3 s3Client,
        IOptions<FileStorageOptions> options,
        ILogger<S3FileStorageService> logger)
    {
        _s3Client = s3Client;
        _s3Settings = options.Value.S3;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string? folder = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                throw new ApplicationArgumentException(Error.Failure("FileStorage.InvalidFile", "File không hợp lệ"));
            }

            // Generate unique filename with normalized original name
            string extension = Path.GetExtension(file.FileName);
            string originalNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
            string normalizedName = NormalizeFileName(originalNameWithoutExt);
            string fileName = $"{normalizedName}_{Guid.NewGuid()}{extension}";
            string key = string.IsNullOrEmpty(folder) ? fileName : $"{folder.Trim('/')}/{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType ?? GetContentType(extension),
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                CannedACL = S3CannedACL.Private
            };

            // Add metadata
            request.Metadata.Add("original-name", file.FileName);
            request.Metadata.Add("uploaded-at", DateTime.UtcNow.ToString("O"));

            PutObjectResponse response = await _s3Client.PutObjectAsync(request, cancellationToken);

            _logger.LogInformation("File uploaded to S3 successfully. Key: {Key}, ETag: {ETag}", key, response.ETag);
            return key; // Return S3 key as file path
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error occurred while uploading file");
            throw new ApplicationInvalidOperationException(
                Error.Failure("FileStorage.S3Error", $"Lỗi S3: {ex.Message}"), ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading file to S3");
            throw new ApplicationInvalidOperationException(
                Error.Failure("FileStorage.SaveFileFailed", "Lưu file thất bại"), ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var request = new DeleteObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = filePath
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
            _logger.LogInformation("File deleted from S3 successfully. Key: {Key}", filePath);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogWarning(ex, "S3 error occurred while deleting file. Key: {Key}", filePath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred while deleting file from S3. Key: {Key}", filePath);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var request = new GetObjectMetadataRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = filePath
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred while checking file existence in S3. Key: {Key}", filePath);
            return false;
        }
    }

    public Uri? GetFileUrl(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        try
        {
            // Return the direct S3 object URL without query parameters
            // Format: https://{bucket}.s3.{region}.amazonaws.com/{key}
            string objectUrl = $"https://{_s3Settings.BucketName}.s3.{_s3Settings.Region ?? "us-east-1"}.amazonaws.com/{filePath}";
            return Uri.TryCreate(objectUrl, UriKind.Absolute, out Uri? uri) ? uri : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred while generating file URL. Key: {Key}", filePath);
            return null;
        }
    }

    /// <summary>
    /// Generate pre-signed URL for secure access
    /// </summary>
    /// <param name="filePath">The file path/key</param>
    /// <returns>Pre-signed URL with query parameters</returns>
    public Uri? GetPresignedFileUrl(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        try
        {
            // Generate pre-signed URL for secure access
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = filePath,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(_s3Settings.PresignedUrlExpiryHours)
            };

            string presignedUrl = _s3Client.GetPreSignedURL(request);
            return Uri.TryCreate(presignedUrl, UriKind.Absolute, out Uri? uri) ? uri : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred while generating presigned URL. Key: {Key}", filePath);
            return null;
        }
    }

    public async Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            var request = new GetObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = filePath
            };

            GetObjectResponse response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred while getting file stream from S3. Key: {Key}", filePath);
            return null;
        }
    }

    /// <summary>
    /// Normalize file name to be safe for storage and URL usage
    /// </summary>
    /// <param name="fileName">Original file name without extension</param>
    /// <returns>Normalized file name</returns>
    private static string NormalizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return "file";
        }

        // Remove diacritics (accents) from Vietnamese and other languages
        string normalized = fileName.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (char c in normalized)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        // Replace spaces and special characters with hyphens
        result = Regex.Replace(result, @"[^\w\-_.]", "-", RegexOptions.Compiled);

        // Remove multiple consecutive hyphens
        result = Regex.Replace(result, @"-+", "-", RegexOptions.Compiled);

        // Remove leading/trailing hyphens
        result = result.Trim('-');

        // Convert to lowercase
        result = result.ToLowerInvariant();

        // Limit length to avoid overly long filenames
        if (result.Length > 50)
        {
            result = result[..50].TrimEnd('-');
        }

        // Ensure we have at least something
        if (string.IsNullOrWhiteSpace(result))
        {
            result = "file";
        }

        return result;
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        ".mp4" => "video/mp4",
        ".webm" => "video/webm",
        ".pdf" => "application/pdf",
        _ => "application/octet-stream"
    };
}
