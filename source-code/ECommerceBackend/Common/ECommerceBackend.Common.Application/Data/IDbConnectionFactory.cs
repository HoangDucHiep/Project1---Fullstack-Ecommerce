using System.Data.Common;

namespace ECommerceBackend.Common.Application.Data;
public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
