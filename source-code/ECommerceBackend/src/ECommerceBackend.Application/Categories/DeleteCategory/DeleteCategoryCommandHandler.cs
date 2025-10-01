using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;


    public DeleteCategoryCommandHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
     
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        // 1. Kiểm tra tồn tại
        const string checkSql = "SELECT COUNT(1) FROM categories WHERE id = @CategoryId";
        int exists = await connection.ExecuteScalarAsync<int>(
            checkSql,
            new { request.CategoryId });
        if (exists == 0)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }
        // Xóa category
        const string deleteSql = "DELETE FROM categories WHERE id = @CategoryId";
        await connection.ExecuteAsync(deleteSql, new { request.CategoryId });

        return Result.Success();
         }
}
