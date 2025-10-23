#pragma warning disable IDE0008 // Use explicit type
using ECommerceBackend.Api.Contracts.Media;
using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Abstracts.FileStorage;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Media;

/// <summary>
/// Controller for handling media upload operations (images and videos)
/// </summary>
[ApiController]
[Route("api/v1/media")]
public class MediaController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(
        IFileStorageService fileStorageService,
        ILogger<MediaController> logger)
    {
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a single media file (image or video)
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="folder">The folder category (e.g., "products", "avatars", "banners")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Media upload response with URL and metadata</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMediaAsync(
        IFormFile file,
        [FromForm] string folder = "products",
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Error.Validation("Media.EmptyFile", "File is required"));
        }

        Result<string> saveResult = await _fileStorageService.SaveFileAsync(file, folder, cancellationToken);

        if (saveResult.IsFailure)
        {
            return StatusCode(saveResult.Error.Type.StatusCode, saveResult.Error);
        }

        var mediaType = DetermineMediaType(file.FileName);

        var response = new MediaUploadResponse(
            MediaUrl: saveResult.Value,
            MediaType: mediaType,
            FileName: file.FileName,
            FileSize: file.Length
        );

        var result = Result.Success(response);
        return Ok(result.ToResponse("Upload media thành công"));
    }

    /// <summary>
    /// Upload multiple media files at once (max 10 files)
    /// </summary>
    /// <param name="files">The files to upload</param>
    /// <param name="folder">The folder category (e.g., "products", "avatars", "banners")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of media upload responses</returns>
    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleMediaAsync(
        List<IFormFile> files,
        [FromForm] string folder = "products",
        CancellationToken cancellationToken = default)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(Error.Validation("Media.EmptyFiles", "At least one file is required"));
        }

        if (files.Count > 10)
        {
            return BadRequest(Error.Validation("Media.TooManyFiles", "Maximum 10 files allowed per upload"));
        }

        var responses = new List<MediaUploadResponse>();
        var errors = new List<Error>();

        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                continue;
            }

            Result<string> saveResult = await _fileStorageService.SaveFileAsync(file, folder, cancellationToken);

            if (saveResult.IsFailure)
            {
                errors.Add(saveResult.Error);
                continue;
            }

            var mediaType = DetermineMediaType(file.FileName);

            responses.Add(new MediaUploadResponse(
                MediaUrl: saveResult.Value,
                MediaType: mediaType,
                FileName: file.FileName,
                FileSize: file.Length
            ));
        }

        // If all uploads failed, return error
        if (responses.Count == 0 && errors.Count > 0)
        {
            return StatusCode(errors[0].Type.StatusCode, errors[0]);
        }

        // Return successful uploads (partial success is still considered success)
        var result = Result.Success(responses);
        return Ok(result.ToResponse($"Upload thành công {responses.Count}/{files.Count} files"));
    }

    /// <summary>
    /// Determines media type based on file extension
    /// </summary>
    private static string DetermineMediaType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => "Image",
            ".mp4" or ".webm" => "Video",
            _ => "Other"
        };
    }
}

