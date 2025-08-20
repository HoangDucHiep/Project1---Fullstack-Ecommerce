using System.Data.Common;

namespace ECommerceBackend.Common.Application.Data;
/// <summary>
/// Abstraction for creating and opening database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Opens and returns a database connection.
    /// </summary>
    ValueTask<DbConnection> OpenConnectionAsync();
}
