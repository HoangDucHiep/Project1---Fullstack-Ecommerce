using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Api.Controllers.Categories;

public sealed class UpdateCategoryRequest
{
    public string Name { get; init; } = null!;
    public CategoryStatus Status { get; init; }
    public string? IconUrl { get; init; }
    public Guid? NewParentId { get; init; }
}
