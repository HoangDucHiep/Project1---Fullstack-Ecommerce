namespace ECommerceBackend.Api.Contracts.Media;

/// <summary>
/// Response for media upload operations
/// </summary>
public record MediaUploadResponse(
    string MediaUrl,
    string MediaType,
    string FileName,
    long FileSize
);

