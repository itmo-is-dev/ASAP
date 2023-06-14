using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Assignments;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetAssignmentsBySubjectCourseTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public GetAssignmentsBySubjectCourseTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectNumberOfAssignments()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Where(x => x.Assignments.Count != 0)
            .FirstAsync();

        var query = new GetAssignmentsBySubjectCourse.Query(subjectCourse.Id);
        var handler = new GetAssignmentsBySubjectCourseHandler(_database.PersistenceContext);

        // Act
        GetAssignmentsBySubjectCourse.Response response = await handler.Handle(query, default);

        // Assert
        response.Assignments.Should().HaveSameCount(subjectCourse.Assignments);
    }
}