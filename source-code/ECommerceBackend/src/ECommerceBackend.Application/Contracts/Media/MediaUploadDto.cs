using ECommerceBackend.Domain.Medias;

namespace ECommerceBackend.Application.Contracts.Media;

public record MediaUploadDto(
    Guid MediaId,
    string FileName,
    string OriginalFileName,
    string FileUrl,
    long FileSize,
    MediaType MediaType,
    bool IsTemp
);
