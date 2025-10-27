#pragma warning disable IDE0008

using ECommerceBackend.Application.Medias;
using FFMpegCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ECommerceBackend.Infrastructure.Medias;

public class MediaMetadataService : IMediaMetadataService
{
    private readonly ILogger<MediaMetadataService> _logger;

    public MediaMetadataService(ILogger<MediaMetadataService> logger)
    {
        _logger = logger;
    }

    public async Task<MediaMetadata> ExtractMetadataAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            var mimeType = file.ContentType.ToLowerInvariant();

            if (mimeType.StartsWith("image/", StringComparison.InvariantCulture))
            {
                return await ExtractImageMetadataAsync(file, cancellationToken);
            }

            if (mimeType.StartsWith("video/", StringComparison.InvariantCulture))
            {
                return await ExtractVideoMetadataAsync(file, cancellationToken);
            }

            return new MediaMetadata(null, null, null, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract metadata from file: {FileName}", file.FileName);
            return new MediaMetadata(null, null, null, null, null);
        }
    }

    public async Task<MediaMetadata> ExtractMetadataAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File not found: {FilePath}", filePath);
                return new MediaMetadata(null, null, null, null, null);
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (IsImageExtension(extension))
            {
                return await ExtractImageMetadataFromFileAsync(filePath, cancellationToken);
            }

            if (IsVideoExtension(extension))
            {
                return await ExtractVideoMetadataFromFileAsync(filePath, cancellationToken);
            }

            return new MediaMetadata(null, null, null, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract metadata from file: {FilePath}", filePath);
            return new MediaMetadata(null, null, null, null, null);
        }
    }

    private async Task<MediaMetadata> ExtractImageMetadataAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream, cancellationToken);

            return new MediaMetadata(
                Width: image.Width,
                Height: image.Height,
                DurationSeconds: null,
                Format: image.Metadata.DecodedImageFormat?.Name,
                Bitrate: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract image metadata from: {FileName}", file.FileName);
            return new MediaMetadata(null, null, null, null, null);
        }
    }

    private async Task<MediaMetadata> ExtractImageMetadataFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            using var image = await Image.LoadAsync(filePath, cancellationToken);

            return new MediaMetadata(
                Width: image.Width,
                Height: image.Height,
                DurationSeconds: null,
                Format: image.Metadata.DecodedImageFormat?.Name,
                Bitrate: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract image metadata from: {FilePath}", filePath);
            return new MediaMetadata(null, null, null, null, null);
        }
    }

    private async Task<MediaMetadata> ExtractVideoMetadataAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            // Create temporary file for FFmpeg processing
            var tempFilePath = Path.GetTempFileName();

            try
            {
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream, cancellationToken);
                }

                return await ExtractVideoMetadataFromFileAsync(tempFilePath, cancellationToken);
            }
            finally
            {
                // Clean up temp file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract video metadata from: {FileName}", file.FileName);
            return new MediaMetadata(null, null, null, null, null);
        }
    }

    private async Task<MediaMetadata> ExtractVideoMetadataFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            var mediaInfo = await FFProbe.AnalyseAsync(filePath, cancellationToken: cancellationToken);
            var videoStream = mediaInfo.VideoStreams.FirstOrDefault();

            return new MediaMetadata(
                Width: videoStream?.Width,
                Height: videoStream?.Height,
                DurationSeconds: (int?)mediaInfo.Duration.TotalSeconds,
                Format: mediaInfo.Format.FormatName,
                Bitrate: (long?)mediaInfo.Format.BitRate
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract video metadata from: {FilePath}", filePath);
            return new MediaMetadata(null, null, null, null, null);
        }
    }

    private static bool IsImageExtension(string extension)
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff" };
        return imageExtensions.Contains(extension);
    }

    private static bool IsVideoExtension(string extension)
    {
        var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm" };
        return videoExtensions.Contains(extension);
    }
}
