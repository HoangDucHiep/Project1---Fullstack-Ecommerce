using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Api.Controllers.Categories.CategoryRegister;

public sealed record CreateCategoryCommand(
    string Name,
    string IconUrl,
    Guid? ParentId,
    int Lft,
    int Rgt,
    int Depth) : ICommand<Guid>;

