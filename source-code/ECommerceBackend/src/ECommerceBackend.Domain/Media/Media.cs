using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Media;
public class Media : Entity
{
    public string FileName { get; private set; }
    public string OriginalFileName { get; private set; }
    public string FilePath { get; private set; }
    public string FileUrl { get; private set; }
    public long FileSize { get; private set; }
    public string MimeType { get; private set; }
    public MediaType MediaType { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public int? Duration { get; private set; } // Seconds for video
    public Guid? UploadedBy { get; private set; }
    public bool IsTemp { get; private set; } = true;
    public DateTimeOffset? ConfirmedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }


    private Media()
    {
        // For EF Core
    }


    public static Media Create(
        string fileName,
        string originalFileName,
        string filePath,
        string fileUrl,
        long fileSize,
        string mimeType,
        MediaType mediaType,
        Guid? uploadedBy = null,
        int? width = null,
        int? height = null,
        int? duration = null)
    {
        return new Media
        {
            FileName = fileName,
            OriginalFileName = originalFileName,
            FilePath = filePath,
            FileUrl = fileUrl,
            FileSize = fileSize,
            MimeType = mimeType,
            MediaType = mediaType,
            Width = width,
            Height = height,
            Duration = duration,
            UploadedBy = uploadedBy,
            IsTemp = true,
            ConfirmedAtUtc = null,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void Confirm()
    {
        IsTemp = false;
        ConfirmedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkAsTemp()
    {
        IsTemp = true;
        ConfirmedAtUtc = null;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateMetadata(int? width, int? height, int? duration)
    {
        Width = width;
        Height = height;
        Duration = duration;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
