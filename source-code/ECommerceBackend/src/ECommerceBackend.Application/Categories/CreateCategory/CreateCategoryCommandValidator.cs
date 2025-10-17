using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Application.Categories.CategoryRegister;
using FluentValidation;


namespace ECommerceBackend.Application.Categories.CategoryRegister;

/// PBNMinh- 08/09/2025
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự.");

        RuleFor(x => x.IconUrl)
            .NotEmpty().WithMessage("Đường dẫn Icon là bắt buộc.")
            .MaximumLength(250).WithMessage("Đường dẫn Icon không được vượt quá 250 ký tự.");


    }
}

