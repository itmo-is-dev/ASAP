using Dapper;
using ITMO.Dev.ASAP.Google.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Google.DataAccess.Models;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;
using System.Data;

namespace ITMO.Dev.ASAP.Google.DataAccess.Repositories;

public class SubjectCourseRepository : ISubjectCourseRepository
{
    private readonly IDbConnection _connection;

    public SubjectCourseRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<GoogleSubjectCourse?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
        select "Id", "SpreadsheetId" from "GoogleSubjectCourse"
        where "Id" = @Id
        """;

        var args = new { Id = id };
        var definition = new CommandDefinition(sql, args, cancellationToken: cancellationToken);

        GoogleSubjectCourseModel? course = await _connection
            .QueryFirstOrDefaultAsync<GoogleSubjectCourseModel>(definition);

        return course?.ToEntity();
    }

    public async Task<IEnumerable<GoogleSubjectCourse>> QueryAsync(
        SubjectCourseQuery query,
        CancellationToken cancellationToken)
    {
        const string sql = """
        select "Id", "SpreadsheetId" from "GoogleSubjectCourse"
        where 
            (cardinality(@Ids) = 0 or "Id" = any(@Ids))
            and (cardinality(@SpreadsheetIds) = 0 or "SpreadsheetId" = any(@SpreadsheetIds))
        """;

        var args = new { query.Ids, query.SpreadsheetIds };
        var definition = new CommandDefinition(sql, args, cancellationToken: cancellationToken);

        IEnumerable<GoogleSubjectCourseModel> courses = await _connection
            .QueryAsync<GoogleSubjectCourseModel>(definition);

        return courses.Select(x => x.ToEntity());
    }

    public async Task AddAsync(GoogleSubjectCourse course, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into "GoogleSubjectCourse" 
        ("Id", "SpreadsheetId") values (@Id, @SpreadsheetId)
        """;

        var args = new { course.Id, course.SpreadsheetId };
        var definition = new CommandDefinition(sql, args, cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(definition);
    }
}