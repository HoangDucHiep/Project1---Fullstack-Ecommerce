using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Api.Controllers.Categories;

/// PBNMinh- 08/09/2025
public sealed record CreateCategoryRequest
(
    string Name,
    string IconUrl,
    Guid? ParentId,
    int Lft,
    int Rgt,
    int Depth
);


