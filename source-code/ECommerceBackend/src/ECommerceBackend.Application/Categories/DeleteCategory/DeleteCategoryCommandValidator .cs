using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ECommerceBackend.Application.Categories.DeleteCategory;
public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
     .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x)
            .Must(x => x.ReplacementCategoryId == null || x.ReplacementCategoryId != x.CategoryId)
            .WithMessage("Replacement category cannot be the same as the category being deleted.");
    }
}
