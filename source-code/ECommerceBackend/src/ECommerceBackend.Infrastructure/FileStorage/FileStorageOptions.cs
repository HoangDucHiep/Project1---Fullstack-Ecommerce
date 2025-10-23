namespace ECommerceBackend.Infrastructure.FileStorage;

/// <summary>
/// Configuration options for file storage.
/// </summary>
public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>
    /// Local storage path (e.g., "wwwroot/uploads")
    /// </summary>
    public string LocalStoragePath { get; set; } = "wwwroot/uploads";

    /// <summary>
    /// Base URL for accessing files (e.g., "http://localhost:5000")
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Maximum image file size in MB
    /// </summary>
    public int MaxImageSizeMB { get; set; } = 5;

    /// <summary>
    /// Maximum video file size in MB
    /// </summary>
    public int MaxVideoSizeMB { get; set; } = 50;

    /// <summary>
    /// Allowed image file extensions
    /// </summary>
    public string[] AllowedImageExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    /// <summary>
    /// Allowed video file extensions
    /// </summary>
    public string[] AllowedVideoExtensions { get; set; } = new[] { ".mp4", ".webm" };
}

