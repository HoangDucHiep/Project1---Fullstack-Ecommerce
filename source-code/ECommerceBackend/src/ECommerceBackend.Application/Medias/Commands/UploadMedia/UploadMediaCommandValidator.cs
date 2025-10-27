using FluentValidation;

namespace ECommerceBackend.Application.Medias.Commands.UploadMedia;


internal sealed class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private static readonly string[] AllowedVideoExtensions = [".mp4", ".avi", ".mov", ".wmv"];
    private static readonly string[] AllowedImageMimeTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];
    private static readonly string[] AllowedVideoMimeTypes = ["video/mp4", "video/avi", "video/quicktime", "video/x-ms-wmv"];

    private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB
    private const long MaxVideoSizeBytes = 100 * 1024 * 1024; // 100MB

    public UploadMediaCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File không được để trống");

        RuleFor(x => x.File.Length)
            .GreaterThan(0)
            .WithMessage("File không được rỗng");

        RuleFor(x => x)
            .Must(HaveValidFileExtension)
            .WithMessage("Định dạng file không được hỗ trợ");

        RuleFor(x => x)
            .Must(HaveValidMimeType)
            .WithMessage("Loại file không được hỗ trợ");

        RuleFor(x => x)
            .Must(HaveValidFileSize)
            .WithMessage("Kích thước file vượt quá giới hạn cho phép");
    }


    private static bool HaveValidFileExtension(UploadMediaCommand command)
    {
        if (command.File == null)
        {
            return false;
        }

        string extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();

        return AllowedImageExtensions.Contains(extension) || AllowedVideoExtensions.Contains(extension);
    }

    private static bool HaveValidMimeType(UploadMediaCommand command)
    {
        if (command.File == null)
        {
            return false;
        }

        string mimeType = command.File.ContentType.ToLowerInvariant();
        return AllowedImageMimeTypes.Contains(mimeType) || AllowedVideoMimeTypes.Contains(mimeType);
    }

    private static bool HaveValidFileSize(UploadMediaCommand command)
    {
        if (command.File == null)
        {
            return false;
        }

        string mimeType = command.File.ContentType.ToLowerInvariant();

        if (AllowedImageMimeTypes.Contains(mimeType))
        {
            return command.File.Length <= MaxImageSizeBytes;
        }

        if (AllowedVideoMimeTypes.Contains(mimeType))
        {
            return command.File.Length <= MaxVideoSizeBytes;
        }

        return false;
    }
}
