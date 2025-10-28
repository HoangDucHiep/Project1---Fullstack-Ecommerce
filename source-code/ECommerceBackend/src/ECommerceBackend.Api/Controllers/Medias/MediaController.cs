using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Application.Contracts.Media;
using ECommerceBackend.Application.Medias.Commands.UploadMedia;
using ECommerceBackend.Application.Medias.Queries.GetMedia;
using ECommerceBackend.Application.Medias.Queries.GetMediaByFileName;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Api.Controllers.Media;


[ApiController]
[Route("api/v1/media")]
public class MediaController : ControllerBase
{
    private readonly ISender _sender;

    public MediaController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Upload a single media file
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Media upload information</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMediaAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var command = new UploadMediaCommand(file);
        Result<MediaUploadDto> result = await _sender.Send(command, cancellationToken);

        object response = result.ToResponse("Upload media thành công");

        return result.IsSuccess
            ? Created($"/api/v1/media/{result.Value.MediaId}", response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    private static readonly SemaphoreSlim _uploadSemaphore = new(initialCount: 3, maxCount: 3); // Limit to 3 concurrent uploads
    /// <summary>
    /// Upload multiple media files
    /// </summary>
    /// <param name="files">The files to upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of media upload information</returns>
    [HttpPost("upload/multiple")]
    public async Task<IActionResult> UploadMultipleMediaAsync(
        List<IFormFile> files,
        CancellationToken cancellationToken = default)
    {
        if (files == null || !files.Any())
        {
            var emptyFilesError = Result.Failure(Error.Validation("Files.Required", "Không có file nào được cung cấp"));
            return BadRequest(emptyFilesError.ToResponse("Upload thất bại"));
        }

        // Upload with limited concurrency
        IEnumerable<Task<Result<MediaUploadDto>>> uploadTasks = files.Select(async file =>
        {
            await _uploadSemaphore.WaitAsync(cancellationToken);
            try
            {
                var command = new UploadMediaCommand(file);
                return await _sender.Send(command, cancellationToken);
            }
            finally
            {
                _uploadSemaphore.Release();
            }
        });

        Result<MediaUploadDto>[] results = await Task.WhenAll(uploadTasks);

        // Rest of the logic remains the same...
        var successResults = results.Where(r => r.IsSuccess).Select(r => r.Value).ToList();
        var failedResults = results.Where(r => !r.IsSuccess).Select(r => r.Error).ToList();

        if (failedResults.Any())
        {
            var multipleUploadError = Error.Failure(
                "Upload.PartialFailure",
                $"Một số file upload thất bại. Thành công: {successResults.Count}, Thất bại: {failedResults.Count}"
            );

            var detailedResponse = new
            {
                Success = false,
                Message = multipleUploadError.Description,
                ErrorCode = multipleUploadError.Code,
                Data = new
                {
                    SuccessCount = successResults.Count,
                    FailedCount = failedResults.Count,
                    SuccessfulUploads = successResults,
                    FailedUploads = failedResults.Select(e => new { e.Code, e.Description }).ToList()
                }
            };

            return BadRequest(detailedResponse);
        }

        var successResult = Result.Success(successResults);
        object response = successResult.ToResponse($"Upload thành công {successResults.Count} files");
        return Ok(response);
    }

    /// <summary>
    /// Get media information by ID
    /// </summary>
    /// <param name="mediaId">Media ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Media information</returns>
    [HttpGet("{mediaId:guid}")]
    public async Task<IActionResult> GetMediaAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMediaQuery(mediaId);
        Result<MediaDto> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy thông tin media thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }

    /// <summary>
    /// Get media file by filename with optional folder path
    /// </summary>
    /// <param name="fileName">File name (can include folder path like "images/photo.jpg")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Media file content with appropriate content type</returns>
    [HttpGet("file/{*fileName}")]
    public async Task<IActionResult> GetMediaFileAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        // Decode URL-encoded filename
        fileName = Uri.UnescapeDataString(fileName);

        var query = new GetMediaByFileNameQuery(fileName);
        Result<MediaDto> result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound($"Media file '{fileName}' not found");
        }

        MediaDto mediaDto = result.Value;

        // Redirect to the actual file URL (S3 or local storage)
        return Redirect(mediaDto.FileUrl);
    }

    /// <summary>
    /// Get media information by filename
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Media information</returns>
    [HttpGet("info/{*fileName}")]
    public async Task<IActionResult> GetMediaInfoByFileNameAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        // Decode URL-encoded filename
        fileName = Uri.UnescapeDataString(fileName);

        var query = new GetMediaByFileNameQuery(fileName);
        Result<MediaDto> result = await _sender.Send(query, cancellationToken);

        object response = result.ToResponse("Lấy thông tin media thành công");

        return result.IsSuccess
            ? Ok(response)
            : StatusCode(result.Error.GetStatusCode(), response);
    }
}
