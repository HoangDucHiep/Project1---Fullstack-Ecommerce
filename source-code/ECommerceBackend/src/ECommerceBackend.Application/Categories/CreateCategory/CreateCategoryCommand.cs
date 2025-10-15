using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Api.Controllers.Categories.CategoryRegister;

/// PBNMinh- 08/09/2025
public sealed record CreateCategoryCommand(
    string Name,
    string IconUrl,
    Guid? ParentId
) : ICommand<Guid>;



