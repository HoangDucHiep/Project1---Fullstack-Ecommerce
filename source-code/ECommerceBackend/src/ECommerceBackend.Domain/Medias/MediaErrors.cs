using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Medias;

public static class MediaErrors
{
    public static Error NotFound => Error.NotFound("Media.NotFound", "Media không tồn tại");
    public static Error NotFoundByIds(IEnumerable<Guid> mediaIds)
    {
        return Error.NotFound("Media.NotFound", $"Media với các ID sau không tồn tại: {string.Join(", ", mediaIds)}");
    }

    public static Error InvalidFileType => Error.Validation("Media.InvalidFileType", "Loại file không được hỗ trợ");
    public static Error InvalidFileSize => Error.Validation("Media.InvalidFileSize", "Kích thước file không hợp lệ");
    public static Error UploadFailed => Error.Failure("Media.UploadFailed", "Upload file thất bại");
    public static Error DeleteFailed => Error.Failure("Media.DeleteFailed", "Xóa file thất bại");
    public static Error AlreadyConfirmed => Error.Validation("Media.AlreadyConfirmed", "Media đã được xác nhận sử dụng");
}
