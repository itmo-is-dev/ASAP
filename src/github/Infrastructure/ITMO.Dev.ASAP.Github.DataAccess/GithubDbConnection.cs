using System.Data;

namespace ITMO.Dev.ASAP.Github.DataAccess;

public class GithubDbConnection
{
    public GithubDbConnection(IDbConnection connection)
    {
        Connection = connection;
    }

    public IDbConnection Connection { get; }
}