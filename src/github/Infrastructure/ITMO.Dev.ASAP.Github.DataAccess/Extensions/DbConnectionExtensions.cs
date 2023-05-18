using System.Data;
using System.Data.Common;

namespace ITMO.Dev.ASAP.Github.DataAccess.Extensions;

public static class DbConnectionExtensions
{
    public static async ValueTask TryOpenAsync(this DbConnection connection, CancellationToken cancellationToken)
    {
        if (connection.State is not ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }
    }
}