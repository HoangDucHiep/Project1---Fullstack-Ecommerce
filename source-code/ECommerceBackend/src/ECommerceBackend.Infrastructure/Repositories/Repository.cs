using ECommerceBackend.Domain.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

/// HDHiep - 09/24/2025
/// <summary>
/// Generic repository base class for managing entities of type <typeparamref name="T"/>.
/// Provides common data access methods such as retrieving all entities, retrieving by ID, and adding new entities.
/// Inherits from the <see cref="Entity"/> base class.
/// </summary>
/// <typeparam name="T">The type of entity managed by the repository. Must inherit from <see cref="Entity"/>.</typeparam>
/// <remarks>
/// This class is intended to be inherited by specific repository implementations for different entity types.
/// It uses Entity Framework Core for data access.
/// </remarks>
public abstract class Repository<T>
    where T : Entity
{
    private readonly ApplicationDbContext _dbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual void Add(T entity)
    {
        _dbContext.Add(entity);
    }
}
