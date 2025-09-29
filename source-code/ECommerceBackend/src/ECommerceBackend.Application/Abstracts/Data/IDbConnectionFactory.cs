using System.Data.Common;

namespace ECommerceBackend.Application.Abstracts.Data;

/// HDHiep - 09/24/2025
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
