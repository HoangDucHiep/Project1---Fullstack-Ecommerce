using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories.Products;



/// <summary>
/// Repository implementation for managing <see cref="ProductOptionType"/> entities.
/// </summary>
public class ProductOptionTypeRepository : Repository<ProductOptionType>, IProductOptionTypeRepository
{
    public ProductOptionTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<ProductOptionType>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductOptionType>()
            .Where(pot => pot.ProductId == productId)
            .OrderBy(pot => pot.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductOptionType?> GetByProductIdAndNameAsync(Guid productId, string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductOptionType>()
            .FirstOrDefaultAsync(pot => pot.ProductId == productId && pot.Name == name, cancellationToken);
    }

    public void Update(ProductOptionType productOptionType)
    {
        _dbContext.Set<ProductOptionType>().Update(productOptionType);
    }
}
