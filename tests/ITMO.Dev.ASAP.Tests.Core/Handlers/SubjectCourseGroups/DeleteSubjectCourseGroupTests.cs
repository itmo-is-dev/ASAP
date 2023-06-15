using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourseGroups;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class DeleteSubjectCourseGroupTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public DeleteSubjectCourseGroupTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveGroupCorrectly()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Where(x => x.SubjectCourseGroups.Count != 0)
            .FirstAsync();

        Guid groupId = subjectCourse.SubjectCourseGroups.First().StudentGroupId;

        var command = new DeleteSubjectCourseGroup.Command(subjectCourse.Id, groupId);
        var handler = new DeleteSubjectCourseGroupHandler(_database.PersistenceContext);

        // Act
        await handler.Handle(command, default);

        // Assert
        subjectCourse.SubjectCourseGroups.Should().NotContain(x => x.StudentGroupId.Equals(groupId));
    }
}