using ECommerceBackend.Application.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Infrastructure.FileStorage;


/// <summary>
/// Factory for creating file storage services based on configuration
/// </summary>
public class FileStorageFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FileStorageOptions _options;

    public FileStorageFactory(IServiceProvider serviceProvider, IOptions<FileStorageOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    public IFileStorageService CreateFileStorageService()
    {
        return _options.StorageType switch
        {
            StorageType.Local => _serviceProvider.GetRequiredService<LocalFileStorageService>(),
            StorageType.S3 => _serviceProvider.GetRequiredService<S3FileStorageService>(),
            _ => throw new NotSupportedException($"Storage type {_options.StorageType} is not supported")
        };
    }
}
