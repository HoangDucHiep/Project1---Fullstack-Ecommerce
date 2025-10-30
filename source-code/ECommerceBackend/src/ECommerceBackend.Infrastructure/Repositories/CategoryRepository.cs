using ECommerceBackend.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

/// HDHiep - 09/27/2025
/// <summary>
/// Repository implementation for managing <see cref="Category"/> entities.
/// Inherits from the generic <see cref="Repository{T}"/> class and implements the <see cref="ICategoryRepository"/> interface.
/// Defines methods for retrieving and adding categories.
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task IncrementAncestorRightValuesAsync(Guid parentId, int shiftBy, CancellationToken cancellationToken)
    {
        const string sql = @"
        UPDATE ""ecommerce-domain"".""categories""
        SET ""rgt"" = ""rgt"" + @p0
        WHERE ""lft"" < (SELECT ""lft"" FROM ""ecommerce-domain"".""categories"" WHERE ""id"" = @p1)
          AND ""rgt"" > (SELECT ""rgt"" FROM ""ecommerce-domain"".""categories"" WHERE ""id"" = @p1);
    ";

        await _dbContext.Database.ExecuteSqlRawAsync(sql, new object[] { shiftBy, parentId }, cancellationToken);
    }
    //PBNMinh
    public async Task ShiftBoundariesAsync(int from, int shiftBy, CancellationToken cancellationToken)
    {
        const string sqlRgt = @"
        UPDATE ""ecommerce-domain"".""categories""
        SET ""rgt"" = ""rgt"" + @p0
        WHERE ""rgt"" >= @p1;
    ";

        const string sqlLft = @"
        UPDATE ""ecommerce-domain"".""categories""
        SET ""lft"" = ""lft"" + @p0
        WHERE ""lft"" > @p1;
    ";

        await _dbContext.Database.ExecuteSqlRawAsync(sqlRgt, new object[] { shiftBy, from }, cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(sqlLft, new object[] { shiftBy, from }, cancellationToken);
    }
    public async Task UpdateChildrenDepthAsync(Guid parentId, int parentDepth, CancellationToken cancellationToken)
    {
        Queue<(Guid ParentId, int Depth)> queue = new Queue<(Guid, int)>();
        queue.Enqueue((parentId, parentDepth));

        while (queue.Count > 0)
        {
            (Guid currentParentId, int currentDepth) = queue.Dequeue();

            List<Category> children = await _dbContext.Set<Category>()
                .Where(c => c.ParentId == currentParentId)
                .ToListAsync(cancellationToken);

            foreach (Category child in children)
            {
                child.MoveTo(child.ParentId, currentDepth + 1);

                queue.Enqueue((child.Id, child.Depth));
            }
        }
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? parentId, Guid excludeId, CancellationToken cancellationToken)
    {
        bool exists = await _dbContext.Set<Category>()
            .AnyAsync(c =>
                c.Name == name &&
                c.ParentId == parentId &&
                c.Id != excludeId,
                cancellationToken);
        return exists;
    }

    public async Task<bool> IsDescendantAsync(Guid ancestorId, Guid descendantId, CancellationToken cancellationToken)
    {
 
        var queue = new Queue<Guid>();
        queue.Enqueue(ancestorId);

        while (queue.Count > 0)
        {
            Guid current = queue.Dequeue();
            List<Category> children = await _dbContext.Set<Category>()
                .Where(c => c.ParentId == current)
                .ToListAsync(cancellationToken);

            foreach (Category child in children)
            {
                if (child.Id == descendantId)
                { return true; }

                queue.Enqueue(child.Id);
            }
        }

        return false;
    }


    public async Task<List<Category>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Category>()
            .Where(c => c.ParentId == parentId)
            .ToListAsync(cancellationToken);
    }

    public override void Delete(Category category)
    {
        _dbContext.Set<Category>().Remove(category);
    }


}
