using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourseGroups;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetSubjectCourseGroupsBySubjectCourseIdTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public GetSubjectCourseGroupsBySubjectCourseIdTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSubjectCourseGroups()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Where(x => x.SubjectCourseGroups.Count != 0)
            .FirstAsync();

        var query = new GetSubjectCourseGroupsBySubjectCourseId.Query(subjectCourse.Id);
        var handler = new GetSubjectCourseGroupsBySubjectCourseIdHandler(_database.PersistenceContext);

        // Act
        GetSubjectCourseGroupsBySubjectCourseId.Response response = await handler.Handle(query, default);

        // Assert
        response.Groups.Should().HaveSameCount(subjectCourse.SubjectCourseGroups);
    }
}