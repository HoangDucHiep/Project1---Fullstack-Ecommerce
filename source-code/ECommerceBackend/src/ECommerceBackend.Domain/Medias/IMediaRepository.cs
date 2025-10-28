namespace ECommerceBackend.Domain.Medias;
public interface IMediaRepository
{
    Task<Media?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Media?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
    Task<Media?> GetByFileUrlAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<List<Media>> GetByFileNamesAsync(List<string> fileNames, CancellationToken cancellationToken = default);
    Task<List<Media>> GetByFileUrlsAsync(List<string> fileUrls, CancellationToken cancellationToken = default);

    Task<List<Media>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Media>> GetTempMediaOlderThan(DateTimeOffset cutoffUtc, CancellationToken cancellationToken = default);
    Task AddAsync(Media media, CancellationToken cancellationToken = default);
    void Update(Media media);
    void Delete(Media media);
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);
}
