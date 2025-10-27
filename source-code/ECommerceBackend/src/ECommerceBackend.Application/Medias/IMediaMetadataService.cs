using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Application.Medias;

public interface IMediaMetadataService
{
    Task<MediaMetadata> ExtractMetadataAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<MediaMetadata> ExtractMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}

public record MediaMetadata(
    int? Width,
    int? Height,
    int? DurationSeconds,
    string? Format,
    long? Bitrate
);
