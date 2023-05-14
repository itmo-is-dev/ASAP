using Dapper;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using System.Runtime.CompilerServices;

namespace ITMO.Dev.ASAP.Github.DataAccess.Repositories;

internal class GithubSubjectCourseRepository : IGithubSubjectCourseRepository
{
    public const string QuerySql = """
    select "Id", "OrganizationName", "TemplateRepositoryName", "MentorTeamName"
    from "GithubSubjectCourses" as gsc
    where 
        (cardinality(@Ids) = 0 or gsc."Id" = any(@Ids))
        and (cardinality(@OrganizationNames) = 0 or
             lower(gsc."OrganizationName") = any(select lower(x) from unnest(@OrganizationNames) as x))
        and (cardinality(@TemplateRepositoryNames) = 0 or
             lower(gsc."TemplateRepositoryName") = any(select lower(x) from unnest(@TemplateRepositoryNames) as x))
        and (cardinality(@MentorTeamNames) = 0 or 
             lower(gsc."MentorTeamName") = any(select lower(x) from unnest(@MentorTeamNames) as x))
    """;

    public const string AddSql = """
    insert into "GithubSubjectCourses"
    ("Id", "OrganizationName", "TemplateRepositoryName", "MentorTeamName")
    values (@Id, @OrganizationName, @TemplateRepositoryName, @MentorTeamName)
    """;

    public const string UpdateSql = """
    update "GithubSubjectCourses"
    set "OrganizationName" = @OrganizationName, 
        "TemplateRepositoryName" = @TemplateRepositoryName, 
        "MentorTeamName" = @MentorTeamName
    where "Id" = @Id
    """;

    private readonly GithubDbConnection _connection;
    private readonly IUnitOfWork _unitOfWork;

    public GithubSubjectCourseRepository(GithubDbConnection connection, IUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async IAsyncEnumerable<GithubSubjectCourse> QueryAsync(
        GithubSubjectCourseQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = QuerySql;

        if (query.Limit is not null)
        {
            sql = $"{sql}\nlimit {query.Limit.Value}";
        }

        var definition = new CommandDefinition(sql, query, cancellationToken: cancellationToken);

        IEnumerable<GithubSubjectCourseModel> assignments = await _connection.Connection
            .QueryAsync<GithubSubjectCourseModel>(definition);

        foreach (GithubSubjectCourseModel assignment in assignments)
        {
            yield return assignment.ToEntity();
        }
    }

    public void Add(GithubSubjectCourse subjectCourse)
    {
        var model = new GithubSubjectCourseModel(
            subjectCourse.Id,
            subjectCourse.OrganizationName,
            subjectCourse.TemplateRepositoryName,
            subjectCourse.MentorTeamName);

        _unitOfWork.Enqueue(AddSql, model);
    }

    public void Update(GithubSubjectCourse subjectCourse)
    {
        var model = new GithubSubjectCourseModel(
            subjectCourse.Id,
            subjectCourse.OrganizationName,
            subjectCourse.TemplateRepositoryName,
            subjectCourse.MentorTeamName);

        _unitOfWork.Enqueue(UpdateSql, model);
    }
}