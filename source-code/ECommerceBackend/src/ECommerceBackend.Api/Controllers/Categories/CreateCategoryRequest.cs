using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Api.Controllers.Categories;

public sealed record CreateCategoryRequest
(
    string Name,
    string IconUrl,
    Guid? ParentId,
    int Lft,
    int Rgt,
    int Depth
);


