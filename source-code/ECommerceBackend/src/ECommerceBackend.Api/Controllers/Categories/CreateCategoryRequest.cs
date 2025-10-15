using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Api.Controllers.Categories;

/// PBNMinh- 08/09/2025
public sealed class CreateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public string IconUrl { get; init; } = string.Empty;
}



