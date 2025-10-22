using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.FileStorage;

/// <summary>
/// Static class containing error definitions for file storage operations.
/// </summary>
public static class FileStorageError
{
    public static Error InvalidFileType(string fileName, string[] allowedExtensions) => Error.Validation(
        "FileStorage.InvalidFileType",
        $"File '{fileName}' has an invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}");

    public static Error FileTooLarge(string fileName, long maxSizeMB) => Error.Validation(
        "FileStorage.FileTooLarge",
        $"File '{fileName}' is too large. Maximum size is {maxSizeMB}MB");

    public static Error FileNotFound(string filePath) => Error.NotFound(
        "FileStorage.FileNotFound",
        $"File '{filePath}' was not found");

    public static Error SaveFailed(string fileName, string reason) => Error.Failure(
        "FileStorage.SaveFailed",
        $"Failed to save file '{fileName}': {reason}");

    public static Error DeleteFailed(string filePath, string reason) => Error.Failure(
        "FileStorage.DeleteFailed",
        $"Failed to delete file '{filePath}': {reason}");

    public static Error EmptyFile(string fileName) => Error.Validation(
        "FileStorage.EmptyFile",
        $"File '{fileName}' is empty");
}

