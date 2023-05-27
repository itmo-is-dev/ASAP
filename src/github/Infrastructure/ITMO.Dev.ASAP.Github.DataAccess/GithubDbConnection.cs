using System.Data.Common;

namespace ITMO.Dev.ASAP.Github.DataAccess;

public class GithubDbConnection : IDisposable
{
    public GithubDbConnection(DbConnection connection)
    {
        Connection = connection;
    }

    public DbConnection Connection { get; }

    public void Dispose()
    {
        Connection.Dispose();
    }
}