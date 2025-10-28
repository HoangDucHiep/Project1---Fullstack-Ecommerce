using ECommerceBackend.Domain.Medias;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

public class MediaRepository : Repository<Media>, IMediaRepository
{
    public MediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Media?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>()
            .FirstOrDefaultAsync(m => m.FileName == fileName, cancellationToken);
    }

    public async Task<Media?> GetByFileUrlAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>()
            .FirstOrDefaultAsync(m => m.FileUrl == fileUrl, cancellationToken);
    }

    public async Task<List<Media>> GetByFileNamesAsync(List<string> fileNames, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>()
            .Where(m => fileNames.Contains(m.FileName))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Media>> GetByFileUrlsAsync(List<string> fileUrls, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>()
            .Where(m => fileUrls.Contains(m.FileUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Media>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>()
            .Where(m => ids.Contains(m.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Media>> GetTempMediaOlderThan(DateTimeOffset cutoffUtc, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>()
            .Where(m => m.IsTemp && m.CreatedAtUtc < cutoffUtc)
            .ToListAsync(cancellationToken);
    }

    public async override Task AddAsync(Media media, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Media>().AddAsync(media, cancellationToken);
    }

    public override void Delete(Media media)
    {
        _dbContext.Set<Media>().Remove(media);
    }

    public async Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Media>().AnyAsync(m => m.Id == id, cancellationToken);
    }

    public void Update(Media media)
    {
        _dbContext.Set<Media>().Update(media);
    }
}
