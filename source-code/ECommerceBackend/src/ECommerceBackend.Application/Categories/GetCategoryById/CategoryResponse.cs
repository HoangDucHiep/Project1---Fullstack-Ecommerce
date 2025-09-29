namespace ECommerceBackend.Application.Categories.GetCategoryById;
public sealed class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string IconUrl { get; set; } = default!;
    public string Status { get; set; } = default!;
    public Guid ParentId { get; set; }
    public int Lft { get; set; }
    public int Rgt { get; set; }
    public int Depth { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }

    // 👇 Dapper cần constructor rỗng
    public CategoryResponse() { }
}
