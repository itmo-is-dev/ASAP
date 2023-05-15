using Dapper;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using System.Runtime.CompilerServices;

namespace ITMO.Dev.ASAP.Github.DataAccess.Repositories;

internal class GithubAssignmentRepository : IGithubAssignmentRepository
{
    private readonly GithubDbConnection _connection;
    private readonly IUnitOfWork _unitOfWork;

    public GithubAssignmentRepository(GithubDbConnection connection, IUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async IAsyncEnumerable<GithubAssignment> QueryAsync(
        GithubAssignmentQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        select ga."Id", ga."SubjectCourseId", ga."BranchName" 
        from "GithubAssignments" as ga
        join "GithubSubjectCourses" as gsc on gsc."Id" = ga."SubjectCourseId"
        where 
            (cardinality(@Ids) = 0 or ga."Id" = any(@Ids))
            and (cardinality(@SubjectCourseIds) = 0 or ga."SubjectCourseId" = any(@SubjectCourseIds))
            and (cardinality(@BranchNames) = 0 or 
                 lower(ga."BranchName") = any(select lower(x) from unnest(@BranchNames) as x))
            and (cardinality(@SubjectCourseOrganizationNames) = 0 or 
                 lower(gsc."OrganizationName") = any(select lower(x) from unnest(@SubjectCourseOrganizationNames) as x))
        """;

        var definition = new CommandDefinition(sql, query, cancellationToken: cancellationToken);

        IEnumerable<GithubAssignmentModel> assignments = await _connection.Connection
            .QueryAsync<GithubAssignmentModel>(definition);

        foreach (GithubAssignmentModel assignment in assignments)
        {
            yield return assignment.ToEntity();
        }
    }

    public void Add(GithubAssignment assignment)
    {
        const string sql = """
        insert into "GithubAssignments"
        ("Id", "SubjectCourseId", "BranchName")
        values (@Id, @SubjectCourseId, @BranchName)
        """;

        var model = new GithubAssignmentModel(assignment.Id, assignment.SubjectCourseId, assignment.BranchName);

        _unitOfWork.Enqueue(sql, model);
    }

    public void Update(GithubAssignment assignment)
    {
        const string sql = """
        update "GithubAssignments"
        set "SubjectCourseId" = @SubjectCourseId, "BranchName" = @BranchName
        where "Id" = @Id
        """;

        var model = new GithubAssignmentModel(assignment.Id, assignment.SubjectCourseId, assignment.BranchName);

        _unitOfWork.Enqueue(sql, model);
    }
}