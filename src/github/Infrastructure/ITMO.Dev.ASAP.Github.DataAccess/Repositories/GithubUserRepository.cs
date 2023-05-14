using Dapper;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Domain.Users;
using System.Runtime.CompilerServices;

namespace ITMO.Dev.ASAP.Github.DataAccess.Repositories;

internal class GithubUserRepository : IGithubUserRepository
{
    private readonly GithubDbConnection _connection;
    private readonly IUnitOfWork _unitOfWork;

    public GithubUserRepository(GithubDbConnection connection, IUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async IAsyncEnumerable<GithubUser> QueryAsync(
        GithubUserQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string baseSql = """
        select "Id", "Username"
        from "GithubUsers" as gu
        where
            (cardinality(@Ids) = 0 or gu."Id" = any(@Ids))
            and (cardinality(@Usernames) = 0 or lower(gu."Username") = any(select lower(x) from unnest(@Usernames) as x))
        """;

        string sql = baseSql;

        if (query.Limit is not null)
        {
            sql = $"{baseSql}\nlimit {query.Limit.Value}";
        }

        // CommandFlags.None for non buffered query
        var definition = new CommandDefinition(
            sql,
            query,
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        IEnumerable<GithubUserModel> submissions = await _connection.Connection
            .QueryAsync<GithubUserModel>(definition);

        foreach (GithubUserModel submission in submissions)
        {
            yield return submission.ToEntity();
        }
    }

    public void AddRange(IReadOnlyCollection<GithubUser> users)
    {
        const string sql = """
        insert into "GithubUsers"("Id", "Username")
        select "Id", "Username"
        from unnest(@Ids, @Usernames) as source("Id", "Username");
        """;

        var args = new
        {
            Ids = users.Select(x => x.Id).ToArray(),
            Usernames = users.Select(x => x.Username).ToArray(),
        };

        _unitOfWork.Enqueue(sql, args);
    }

    public void Add(GithubUser user)
    {
        const string sql = """
        insert into "GithubUsers" ("Id", "Username")
        values (@Id, @Username)
        """;

        var model = new GithubUserModel(user.Id, user.Username);

        _unitOfWork.Enqueue(sql, model);
    }

    public void Update(GithubUser user)
    {
        const string sql = """
        update "GithubUsers"
        set "Username" = @Username
        where "Id" = @Id
        """;

        var model = new GithubUserModel(user.Id, user.Username);

        _unitOfWork.Enqueue(sql, model);
    }
}