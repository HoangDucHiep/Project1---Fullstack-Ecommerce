using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Application.FileStorage;
public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string? folder = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
    Uri? GetFileUrl(string filePath);
    Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken cancellationToken = default);
}
