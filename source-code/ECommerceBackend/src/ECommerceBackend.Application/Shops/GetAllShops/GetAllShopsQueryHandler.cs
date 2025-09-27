using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.GetAllShops;
internal sealed class GetAllShopsQueryHandler : IQueryHandler<GetAllShopsQuery, List<Shop>>
{

    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetAllShopsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<List<Shop>>> Handle(GetAllShopsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection dbConnection = await _dbConnectionFactory.OpenConnectionAsync();

        string sql = """
            SELECT * FROM shops   
        """;

        IEnumerable<Shop> shops = await dbConnection.QueryAsync<Shop>(sql);
        return shops.ToList();
    }
}
