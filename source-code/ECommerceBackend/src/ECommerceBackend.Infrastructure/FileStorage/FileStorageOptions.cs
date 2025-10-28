namespace ECommerceBackend.Infrastructure.FileStorage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    // Local storage (keep for backward compatibility)
    public string LocalStoragePath { get; set; } = "wwwroot/uploads";
    public string BaseUrl { get; set; } = "http://localhost:5000";

    // S3 Configuration
    public S3Settings S3 { get; set; } = new();

    // File validation
    public int MaxImageSizeMB { get; set; } = 5;
    public int MaxVideoSizeMB { get; set; } = 50;
    public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    public string[] AllowedVideoExtensions { get; set; } = [".mp4", ".webm"];

    // Storage type selection
    public StorageType StorageType { get; set; } = StorageType.S3;
}

public class S3Settings
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "ap-southeast-1";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty; // For LocalStack
    public bool UseLocalStack { get; set; }
    public int PresignedUrlExpiryHours { get; set; } = 24;
}

public enum StorageType
{
    Local,
    S3
}
