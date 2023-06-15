using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourses;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class UpdateSubjectCourseTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public UpdateSubjectCourseTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateSubjectCourse()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .FirstAsync();

        string newTitle = subjectCourse.Title + "_new";

        var command = new UpdateSubjectCourse.Command(subjectCourse.Id, newTitle);
        var handler = new UpdateSubjectCourseHandler(_database.PersistenceContext);

        // Act
        await handler.Handle(command, default);

        // Assert
        subjectCourse.Title.Should().Be(newTitle);
    }
}