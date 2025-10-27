using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;
using ECommerceBackend.Application.FileStorage;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Medias;

namespace ECommerceBackend.Application.Medias.Commands.UploadMedia;

public class UploadMediaCommandHandler : ICommandHandler<UploadMediaCommand, MediaUploadDto>
{
    private const string MEDIA_FOLDER = "media";

    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaMetadataService _mediaMetadataService;

    public UploadMediaCommandHandler(
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        IMediaRepository mediaRepository,
        IMediaMetadataService mediaMetadataService
    )
    {
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _mediaRepository = mediaRepository;
        _mediaMetadataService = mediaMetadataService;

    }

    public async Task<Result<MediaUploadDto>> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Save file to storage using IFileStorageService
            string filePath = await _fileStorageService.SaveFileAsync(request.File, MEDIA_FOLDER, cancellationToken);
            string fileUrl = _fileStorageService.GetFileUrl(filePath)?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(fileUrl))
            {
                return Result.Failure<MediaUploadDto>(MediaErrors.UploadFailed);
            }

            // 2. Determine media type
            MediaType mediaType = DetermineMediaType(request.File.ContentType);

            // 3. Extract metadata using MediaMetadataService
            MediaMetadata metadata = await _mediaMetadataService.ExtractMetadataAsync(request.File, cancellationToken);

            // 4. Create Media entity
            var media = Domain.Medias.Media.Create(
                fileName: Path.GetFileName(filePath),
                originalFileName: request.File.FileName,
                filePath: filePath,
                fileUrl: fileUrl.ToString(),
                fileSize: request.File.Length,
                mimeType: request.File.ContentType,
                mediaType: mediaType,
                uploadedBy: request.UploadedBy,
                width: metadata.Width,
                height: metadata.Height,
                duration: metadata.DurationSeconds
            );

            // 5. Save to database
            await _mediaRepository.AddAsync(media, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Return DTO
            return new MediaUploadDto(
                media.Id,
                media.FileName,
                media.OriginalFileName,
                media.FileUrl,
                media.FileSize,
                media.MediaType,
                media.IsTemp
            );
        }
        catch (Exception)
        {
            return Result.Failure<MediaUploadDto>(MediaErrors.UploadFailed);
        }
    }

    private static MediaType DetermineMediaType(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            var mt when mt.StartsWith("image/", StringComparison.Ordinal) => MediaType.Image,
            var mt when mt.StartsWith("video/", StringComparison.Ordinal) => MediaType.Video,
            var mt when mt.StartsWith("application/pdf", StringComparison.Ordinal) => MediaType.Document,
            _ => MediaType.Other
        };
    }
}
