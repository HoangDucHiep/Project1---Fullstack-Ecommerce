using System.Data.Common;
using ECommerceBackend.Application.Abstracts.Data;
using Npgsql;

namespace ECommerceBackend.Infrastructure.Data;


/// HDHiep - 09/24/2025
/// <summary>
/// PostgreSQL implementation of <see cref="IDbConnectionFactory"/> using <see cref="NpgsqlDataSource"/>.
/// </summary>
internal sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class.
    /// </summary>
    /// <param name="dataSource">The configured <see cref="NpgsqlDataSource"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataSource"/> is null.</exception>
    public DbConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <summary>
    /// Opens and returns a new database connection.
    /// </summary>
    /// <returns>An open <see cref="DbConnection"/>.</returns>
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        return await _dataSource.OpenConnectionAsync();
    }
}
