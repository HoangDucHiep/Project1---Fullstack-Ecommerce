using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.CategoryRegister;

public sealed class CreateCategoryCommandHandler
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        // if (!_userContext.IsAuthenticated)
        // {
        //     return Result.Failure<Guid>(CategoryErrors.AccessDenied("create category"));
        // }

        // 1️⃣ Lấy danh sách category hiện có
        List<Category> categories = await _categoryRepository.GetAllAsync(cancellationToken);

        // 2️⃣ Kiểm tra trùng tên trong cùng cấp
        bool isDuplicate = categories.Any(c =>
            c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase) &&
            c.ParentId == command.ParentId);

        if (isDuplicate)
        {
            return Result.Failure<Guid>(
                CategoryErrors.DuplicateName(command.Name, command.ParentId)
            );
        }

        // ====== CASE 1️⃣: ROOT NODE ======
        if (!command.ParentId.HasValue)
        {
            int maxRight = categories.Any() ? categories.Max(c => c.Rgt) : 0;

            var root = Category.Create(
                name: command.Name,
                iconUrl: command.IconUrl,
                parentId: null,
                lft: maxRight + 1,
                rgt: maxRight + 2,
                depth: 0
            );

            _categoryRepository.Add(root);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(root.Id);
        }

        // ====== CASE 2️⃣: CHILD NODE ======
        Category? parent = await _categoryRepository.GetByIdAsync(command.ParentId.Value, cancellationToken);
        if (parent is null)
        {
            return Result.Failure<Guid>(
                CategoryErrors.InvalidParent(Guid.Empty, command.ParentId.Value)
            );
        }

        // Vị trí chèn = Right của cha
        int insertPosition = parent.Rgt;
        int newDepth = parent.Depth + 1;

        // B1️⃣. Dịch tất cả node có Left/Right >= insertPosition để tạo khoảng trống
        await _categoryRepository.ShiftBoundariesAsync(insertPosition, 2, cancellationToken);

        // B2️⃣. Tạo node mới nằm ngay trong khoảng trống vừa tạo
        var category = Category.Create(
            name: command.Name,
            iconUrl: command.IconUrl,
            parentId: parent.Id,
            lft: insertPosition,
            rgt: insertPosition + 1,
            depth: newDepth
        );

        // B3️⃣. Thêm node mới vào DB
        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(category.Id);
    }
}
