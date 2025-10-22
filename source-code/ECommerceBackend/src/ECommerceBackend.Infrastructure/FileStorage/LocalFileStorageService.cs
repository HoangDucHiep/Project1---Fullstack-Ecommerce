#pragma warning disable IDE0008 // Use explicit type
using ECommerceBackend.Application.Abstracts.FileStorage;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Infrastructure.FileStorage;

/// <summary>
/// Local file storage implementation.
/// Stores files on the local file system in the wwwroot/uploads directory.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IOptions<FileStorageOptions> options,
        IWebHostEnvironment environment,
        ILogger<LocalFileStorageService> logger)
    {
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task<Result<string>> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate file is not null or empty
            if (file == null || file.Length == 0)
            {
                return Result.Failure<string>(FileStorageError.EmptyFile(file?.FileName ?? "unknown"));
            }

            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            // Determine if it's an image or video and validate accordingly
            var isImage = _options.AllowedImageExtensions.Contains(fileExtension);
            var isVideo = _options.AllowedVideoExtensions.Contains(fileExtension);

            if (!isImage && !isVideo)
            {
                var allAllowedExtensions = _options.AllowedImageExtensions
                    .Concat(_options.AllowedVideoExtensions)
                    .ToArray();
                return Result.Failure<string>(FileStorageError.InvalidFileType(fileName, allAllowedExtensions));
            }

            // Validate file size
            var maxSizeBytes = isImage
                ? _options.MaxImageSizeMB * 1024 * 1024
                : _options.MaxVideoSizeMB * 1024 * 1024;

            if (file.Length > maxSizeBytes)
            {
                var maxSizeMB = isImage ? _options.MaxImageSizeMB : _options.MaxVideoSizeMB;
                return Result.Failure<string>(FileStorageError.FileTooLarge(fileName, maxSizeMB));
            }

            // Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            // Create full directory path
            var uploadPath = Path.Combine(_environment.ContentRootPath, _options.LocalStoragePath, folder);
            
            // Ensure directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Full file path
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Save file to disk
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // Return relative path for database storage
            var relativePath = $"/uploads/{folder}/{uniqueFileName}";

            _logger.LogInformation("File saved successfully: {FilePath}", relativePath);

            return Result.Success(relativePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FileName}", file?.FileName);
            return Result.Failure<string>(FileStorageError.SaveFailed(file?.FileName ?? "unknown", ex.Message));
        }
    }

    public async Task<Result> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Failure(FileStorageError.FileNotFound(filePath ?? "null"));
            }

            // Convert relative path to absolute path
            var absolutePath = Path.Combine(_environment.ContentRootPath, _options.LocalStoragePath.Replace("wwwroot/", ""), filePath.TrimStart('/'));

            if (!File.Exists(absolutePath))
            {
                return Result.Failure(FileStorageError.FileNotFound(filePath));
            }

            await Task.Run(() => File.Delete(absolutePath), cancellationToken);

            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return Result.Failure(FileStorageError.DeleteFailed(filePath, ex.Message));
        }
    }

    public Uri? GetFileUrl(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        // If it's already a full URL, return as is
        if (Uri.TryCreate(filePath, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri;
        }

        // Combine base URL with relative path
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var path = filePath.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";

        return Uri.TryCreate(fullUrl, UriKind.Absolute, out var uri) ? uri : null;
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.FromResult(false);
            }

            // Convert relative path to absolute path
            // filePath = "/uploads/products/abc.jpg"
            // We need to combine with ContentRootPath + "wwwroot" + filePath
            var absolutePath = Path.Combine(_environment.ContentRootPath, "wwwroot", filePath.TrimStart('/'));

            return Task.FromResult(File.Exists(absolutePath));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}

