namespace ECommerceBackend.Domain.Categories;
public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Category category);

    Task IncrementAncestorRightValuesAsync(Guid parentId, int shiftBy, CancellationToken cancellationToken);
    Task UpdateChildrenDepthAsync(Guid parentId, int parentDepth, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, Guid? parentId, Guid excludeId, CancellationToken cancellationToken);
    Task<bool> IsDescendantAsync(Guid ancestorId, Guid descendantId, CancellationToken cancellationToken);

    Task<List<Category>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    void Delete(Category category);



}


