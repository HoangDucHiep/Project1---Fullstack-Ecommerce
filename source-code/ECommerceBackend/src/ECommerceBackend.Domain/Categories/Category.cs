using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Abstracts.Utils;

namespace ECommerceBackend.Domain.Categories;

public enum CategoryStatus
{
    ACTIVE,
    INACTIVE,
    DELETED
}

public class Category : Entity
{
    public string Name { get; private set; }
    public string IconUrl { get; private set; }
    public CategoryStatus Status { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Lft { get; private set; }
    public int Rgt { get; private set; }
    public int Depth { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private Category()
    {
        // For ORM
    }
    //PBNMinh - 08/10/2025
    public static Category Create(string name, string iconUrl, Guid? parentId, int depth)
    {
        var category = new Category
        {
            Id = IdGenerator.GenerateId(),
            Name = name,
            IconUrl = iconUrl,
            Status = CategoryStatus.ACTIVE,
            ParentId = parentId,
            Depth = depth,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        // Raise domain event if needed

        return category;
    }

    public void Update(string name, string iconUrl)
    {
        Name = name;
        IconUrl = iconUrl;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        // Raise domain event if needed
    }

    public void ChangeStatus(CategoryStatus newStatus)
    {
        Status = newStatus;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        // Raise domain event if needed
    }


    public void Rename(string name)
    {
        Name = name;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void MoveTo(Guid? newParentId, int newDepth)
    {
        ParentId = newParentId;
        Depth = newDepth;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

}
