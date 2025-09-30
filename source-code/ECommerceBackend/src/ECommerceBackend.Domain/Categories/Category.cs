using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Utils;

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

    public static Category Create(string name, string iconUrl, Guid? parentId, int lft, int rgt, int depth)
    {
        var category = new Category
        {
            Id = IdGenerator.GenerateId(),
            Name = name,
            IconUrl = iconUrl,
            Status = CategoryStatus.ACTIVE,
            ParentId = parentId,
            Lft = lft,
            Rgt = rgt,
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

    public void MoveCategory(Guid? newParentId, int newLft, int newRgt, int newDepth)
    {
        ParentId = newParentId;
        Lft = newLft;
        Rgt = newRgt;
        Depth = newDepth;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        // Raise domain event if needed
    }

}
