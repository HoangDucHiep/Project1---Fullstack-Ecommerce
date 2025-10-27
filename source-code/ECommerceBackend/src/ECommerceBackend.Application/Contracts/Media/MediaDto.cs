using ECommerceBackend.Domain.Medias;

namespace ECommerceBackend.Application.Contracts.Media;


public record MediaDto(
    Guid Id,
    string FileName,
    string OriginalFileName,
    string FileUrl,
    long FileSize,
    string MimeType,
    MediaType MediaType,
    int? Width,
    int? Height,
    int? Duration,
    bool IsTemp,
    DateTimeOffset? ConfirmedAtUtc,
    DateTimeOffset CreatedAtUtc
)
{
    public static MediaDto FromEntity(Domain.Medias.Media media) => new(
        media.Id,
        media.FileName,
        media.OriginalFileName,
        media.FileUrl,
        media.FileSize,
        media.MimeType,
        media.MediaType,
        media.Width,
        media.Height,
        media.Duration,
        media.IsTemp,
        media.ConfirmedAtUtc,
        media.CreatedAtUtc
    );
}
