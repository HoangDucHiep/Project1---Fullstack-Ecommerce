using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Application.FileStorage;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Infrastructure.FileStorage;


/// HDHiep - 10/25/2025
/// <summary>
/// Implementation of IFileStorageService that stores files on the local file system.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageOptions _options;

    public LocalFileStorageService(IWebHostEnvironment environment, FileStorageOptions options)
    {
        _environment = environment;
        _options = options;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string? folder = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                throw new ApplicationArgumentException(Error.Failure("FileStorage.InvalidFile", "File không hợp lệ"));
            }


            string folderPath = folder ?? _options.LocalStoragePath;
            string fileName = $"{Guid.NewGuid()}{file.FileName}";
            string relativePath = Path.Combine(folderPath, fileName).Replace('\\', '/');
            string absolutePath = Path.Combine(_environment.WebRootPath, folderPath);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(absolutePath);

            string fullPath = Path.Combine(absolutePath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/{relativePath}";

        }
        catch (Exception ex)
        {
            throw new ApplicationInvalidOperationException(Error.Failure("FileStorage.SaveFileFailed", "Lưu file thất bại"), ex);
        }
    }
    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.FromResult(false);
            }

            string absolutePath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.FromResult(false);
            }

            string absolutePath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            return Task.FromResult(File.Exists(absolutePath));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Uri? GetFileUrl(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        string baseUrl = _options.BaseUrl.TrimEnd('/');
        string cleanPath = filePath.TrimStart('/');
        string fullUrl = $"{baseUrl}/{cleanPath}";

        return Uri.TryCreate(fullUrl, UriKind.Absolute, out Uri? uri) ? uri : null;
    }

    public Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.FromResult<Stream?>(null);
            }

            string absolutePath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (!File.Exists(absolutePath))
            {
                return Task.FromResult<Stream?>(null);
            }

            var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
            return Task.FromResult<Stream?>(stream);
        }
        catch
        {
            return Task.FromResult<Stream?>(null);
        }
    }
}
