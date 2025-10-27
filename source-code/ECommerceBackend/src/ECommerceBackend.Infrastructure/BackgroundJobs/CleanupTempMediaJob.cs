using ECommerceBackend.Application.FileStorage;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Medias;
using Microsoft.Extensions.Logging;

namespace ECommerceBackend.Infrastructure.BackgroundJobs;


public class CleanupTempMediaJob
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CleanupTempMediaJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CleanupTempMediaJob(
        IMediaRepository mediaRepository,
        IFileStorageService fileStorageService,
        ILogger<CleanupTempMediaJob> logger,
        IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Starting cleanup of temporary media files");

            DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.AddHours(-24);
            List<Domain.Medias.Media> tempMediaToDelete = await _mediaRepository.GetTempMediaOlderThan(cutoffTime);

            int deletedCount = 0;
            int failedCount = 0;

            foreach (Domain.Medias.Media media in tempMediaToDelete)
            {
                try
                {
                    // Delete physical file
                    bool fileDeleted = await _fileStorageService.DeleteFileAsync(media.FilePath);

                    if (fileDeleted)
                    {
                        // Delete database record
                        _mediaRepository.Delete(media);
                        await _unitOfWork.SaveChangesAsync();
                        deletedCount++;

                        _logger.LogDebug("Deleted temp media: {MediaId} - {FileName}", media.Id, media.FileName);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to delete physical file for media: {MediaId} - {FilePath}", media.Id, media.FilePath);
                        failedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting temp media: {MediaId} - {FileName}", media.Id, media.FileName);
                    failedCount++;
                }
            }

            _logger.LogInformation("Cleanup completed. Deleted: {DeletedCount}, Failed: {FailedCount}", deletedCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during temp media cleanup");
        }
    }
}
