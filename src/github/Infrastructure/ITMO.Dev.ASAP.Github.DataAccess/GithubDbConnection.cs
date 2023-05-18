using System.Data.Common;

namespace ITMO.Dev.ASAP.Github.DataAccess;

public class GithubDbConnection
{
    public GithubDbConnection(DbConnection connection)
    {
        Connection = connection;
    }

    public DbConnection Connection { get; }
}