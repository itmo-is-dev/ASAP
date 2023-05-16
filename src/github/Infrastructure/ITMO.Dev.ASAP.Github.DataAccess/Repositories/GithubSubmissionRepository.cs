using Dapper;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using System.Runtime.CompilerServices;
using System.Text;

namespace ITMO.Dev.ASAP.Github.DataAccess.Repositories;

internal class GithubSubmissionRepository : IGithubSubmissionRepository
{
    private readonly GithubDbConnection _connection;
    private readonly IUnitOfWork _unitOfWork;

    public GithubSubmissionRepository(GithubDbConnection connection, IUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async IAsyncEnumerable<GithubSubmission> QueryAsync(
        GithubSubmissionQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string baseSql = """
        select gs."Id", gs."AssignmentId", gs."UserId", gs."CreatedAt", gs."Organization", gs."Repository", gs."PullRequestNumber"
        from "GithubSubmissions" as gs
        join "GithubAssignments" as ga on gs."AssignmentId" = ga."Id"
        where 
            (cardinality(@Ids) = 0 or gs."Id" = any(@Ids))
            and (cardinality(@RepositoryNames) = 0 or gs."Repository" = any(@RepositoryNames))
            and (cardinality(@PullRequestNumbers) = 0 or gs."PullRequestNumber" = any(@PullRequestNumbers))
            and (cardinality(@OrganizationNames) = 0 or gs."Organization" = any(@OrganizationNames))
            and (cardinality(@AssignmentBranchNames) or ga."BranchName" = any(@AssignmentBranchNames))
        """;

        string sql = baseSql;

        if (query.HasOrderParameters)
        {
            var orders = new List<string?>
            {
                query.OrderByCreatedAt switch
                {
                    OrderDirection.Ascending => "\"CreatedAt\" asc",
                    OrderDirection.Descending => "\"CreatedAt\" desc",
                    _ => null,
                },
            };

            var builder = new StringBuilder(baseSql);
            builder.AppendLine();
            builder.Append("order by ");
            builder.AppendJoin(", ", orders.Where(x => string.IsNullOrEmpty(x) is false));

            sql = builder.ToString();
        }

        // CommandFlags.None for non buffered query
        var definition = new CommandDefinition(
            sql,
            query,
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        IEnumerable<GithubSubmissionModel> submissions = await _connection.Connection
            .QueryAsync<GithubSubmissionModel>(definition);

        foreach (GithubSubmissionModel submission in submissions)
        {
            yield return submission.ToEntity();
        }
    }

    public void Add(GithubSubmission submission)
    {
        const string sql = """
        insert into "GithubSubmissions"
        ("Id", "AssignmentId", "UserId", "CreatedAt", "Organization", "Repository", "PullRequestNumber")
        values (@Id, @AssignmentId, @UserId, @CreatedAt, @Organization, @Repository, @PullRequestNumber)
        """;

        var model = new GithubSubmissionModel(
            submission.Id,
            submission.AssignmentId,
            submission.UserId,
            submission.CreatedAt,
            submission.Organization,
            submission.Repository,
            submission.PullRequestNumber);

        _unitOfWork.Enqueue(sql, model);
    }
}