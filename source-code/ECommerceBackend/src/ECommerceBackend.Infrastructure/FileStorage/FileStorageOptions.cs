namespace ECommerceBackend.Infrastructure.FileStorage;


public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string LocalStoragePath { get; set; } = "uploads";
    public string BaseUrl { get; set; } = "https://localhost:5001";
    public int MaxImageSizeMB { get; set; } = 10;
    public int MaxVideoSizeMB { get; set; } = 100;
    public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    public string[] AllowedVideoExtensions { get; set; } = [".mp4", ".avi", ".mov", ".wmv"];
}
