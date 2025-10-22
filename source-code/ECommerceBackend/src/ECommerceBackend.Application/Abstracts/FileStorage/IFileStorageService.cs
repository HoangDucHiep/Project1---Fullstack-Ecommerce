using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Application.Abstracts.FileStorage;

/// <summary>
/// Service interface for handling file storage operations.
/// This abstraction allows for easy switching between local storage and cloud storage providers.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to the storage system.
    /// </summary>
    /// <param name="file">The file to save.</param>
    /// <param name="folder">The folder/category where the file should be stored (e.g., "products", "avatars", "banners").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the relative file path if successful, or an error.</returns>
    Task<Result<string>> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage system.
    /// </summary>
    /// <param name="filePath">The relative path of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<Result> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full URL for accessing a file.
    /// </summary>
    /// <param name="filePath">The relative file path.</param>
    /// <returns>The full URL to access the file.</returns>
    Uri? GetFileUrl(string filePath);

    /// <summary>
    /// Checks if a file exists in the storage system.
    /// </summary>
    /// <param name="filePath">The relative path of the file to check.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    Task<bool> FileExistsAsync(string filePath);
}

