using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler
    : ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
        { return Result.Failure(CategoryErrors.NotFound(command.CategoryId)); }

        if (category.ParentId is null)
        { return Result.Failure(CategoryErrors.CannotDeleteRootCategory(category.Id)); }

        List<Category> children = await _categoryRepository.GetChildrenAsync(category.Id, cancellationToken);
        if (children.Any() && !command.Cascade)
        { return Result.Failure(CategoryErrors.HasChildren(category.Id)); }

        if (command.Cascade)
        {
            foreach (Category child in children)
            {
                var cascadeCmd = new DeleteCategoryCommand(child.Id, command.ReplacementCategoryId, true);
                Result result = await Handle(cascadeCmd, cancellationToken);
                if (result.IsFailure)
                { return result; }
            }
        }

        List<Product> products = await _productRepository.GetByCategoryIdAsync(category.Id, cancellationToken);
        if (products.Any())
        {
            if (command.ReplacementCategoryId is null)
            { return Result.Failure(CategoryErrors.ReplacementRequired(category.Id)); }

            if (command.ReplacementCategoryId == category.Id)
            { return Result.Failure(CategoryErrors.InvalidReplacement(category.Id)); }

            Category? replacement = await _categoryRepository.GetByIdAsync(command.ReplacementCategoryId.Value, cancellationToken);
            if (replacement is null)
            { return Result.Failure(CategoryErrors.InvalidReplacement(command.ReplacementCategoryId.Value)); }

            foreach (Product product in products)
            {
                product.ChangeCategory(replacement.Id);
                _productRepository.Update(product);
            }
        }

        _categoryRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
