
using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;
