using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;
using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Application.Medias.Commands.UploadMedia;

public record UploadMediaCommand(
    IFormFile File,
    Guid? UploadedBy = null
) : ICommand<MediaUploadDto>;
