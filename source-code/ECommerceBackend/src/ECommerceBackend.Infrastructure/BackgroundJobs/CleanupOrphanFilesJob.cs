#pragma warning disable IDE0008 // Use explicit type
using ECommerceBackend.Domain.Products;
using ECommerceBackend.Infrastructure.FileStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job to clean up orphan files (files that are not referenced in the database)
/// Runs daily at 2:00 AM to remove files older than 24 hours that are not being used
/// </summary>
public sealed class CleanupOrphanFilesJob
{
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageOptions _options;
    private readonly ILogger<CleanupOrphanFilesJob> _logger;

    public CleanupOrphanFilesJob(
        IProductMediaRepository productMediaRepository,
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options,
        ILogger<CleanupOrphanFilesJob> logger)
    {
        _productMediaRepository = productMediaRepository;
        _environment = environment;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Executes the cleanup job
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting orphan files cleanup job...");

        try
        {
            var uploadPath = Path.Combine(_environment.ContentRootPath, _options.LocalStoragePath);

            if (!Directory.Exists(uploadPath))
            {
                _logger.LogInformation("Upload directory does not exist. Nothing to clean up.");
                return;
            }

            // Get all media URLs from the database
            List<ProductMedia> allProductMedia = await _productMediaRepository.GetAllAsync(cancellationToken);
            var usedMediaUrls = allProductMedia.Select(m => m.MediaUrl).ToHashSet();

            _logger.LogInformation("Found {Count} media files in database", usedMediaUrls.Count);

            // Get all files in the upload directory
            var allFiles = Directory.GetFiles(uploadPath, "*.*", SearchOption.AllDirectories);
            var deletedCount = 0;
            var totalSize = 0L;

            foreach (var filePath in allFiles)
            {
                try
                {
                    // Get relative path for comparison
                    var relativePath = GetRelativePath(uploadPath, filePath);

                    // Check if file is referenced in database
                    if (usedMediaUrls.Contains(relativePath))
                    {
                        continue; // File is being used, skip it
                    }

                    // Check if file is older than 24 hours
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.CreationTimeUtc > DateTime.UtcNow.AddHours(-24))
                    {
                        continue; // File is too new, might be in the process of being used
                    }

                    // Delete the orphan file
                    var fileSize = fileInfo.Length;
                    File.Delete(filePath);
                    deletedCount++;
                    totalSize += fileSize;

                    _logger.LogInformation("Deleted orphan file: {FilePath} ({Size} bytes)", relativePath, fileSize);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                }
            }

            _logger.LogInformation(
                "Orphan files cleanup completed. Deleted {Count} files, freed {Size} bytes",
                deletedCount,
                totalSize);

            // Clean up empty directories
            CleanupEmptyDirectories(uploadPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during orphan files cleanup job");
        }
    }

    /// <summary>
    /// Gets the relative path from the upload directory
    /// </summary>
    private static string GetRelativePath(string basePath, string fullPath)
    {
        var relativePath = fullPath.Replace(basePath, "").Replace(Path.DirectorySeparatorChar, '/');
        if (!relativePath.StartsWith('/'))
        {
            relativePath = "/" + relativePath;
        }
        return "/uploads" + relativePath;
    }

    /// <summary>
    /// Recursively removes empty directories
    /// </summary>
    private void CleanupEmptyDirectories(string directory)
    {
        try
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                CleanupEmptyDirectories(subDirectory);

                // If directory is empty after cleanup, delete it
                if (!Directory.EnumerateFileSystemEntries(subDirectory).Any())
                {
                    Directory.Delete(subDirectory);
                    _logger.LogInformation("Deleted empty directory: {Directory}", subDirectory);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up empty directories in: {Directory}", directory);
        }
    }
}

