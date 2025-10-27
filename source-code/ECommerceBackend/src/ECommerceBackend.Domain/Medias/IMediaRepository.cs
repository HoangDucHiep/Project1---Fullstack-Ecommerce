namespace ECommerceBackend.Domain.Medias;
public interface IMediaRepository
{
    Task<Media?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Media>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Media>> GetTempMediaOlderThan(DateTimeOffset cutoffUtc, CancellationToken cancellationToken = default);
    Task AddAsync(Media media, CancellationToken cancellationToken = default);
    void Update(Media media);
    void Delete(Media media);
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);

}
