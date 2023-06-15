using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourseGroups;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateSubjectCourseGroupTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public CreateSubjectCourseGroupTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateSubjectCourseGroupCorrectly()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Where(x => x.Assignments.Count != 0)
            .FirstAsync();

        var studentGroup = new StudentGroupModel(_database.Faker.Random.Guid(), _database.Faker.Commerce.ProductName());

        _database.Context.StudentGroups.Add(studentGroup);
        await _database.Context.SaveChangesAsync();

        var publisher = new Mock<IPublisher>();

        var command = new CreateSubjectCourseGroup.Command(subjectCourse.Id, studentGroup.Id);
        var handler = new CreateSubjectCourseGroupHandler(_database.PersistenceContext, publisher.Object);

        // Act
        await handler.Handle(command, default);

        subjectCourse = await _database.Context.SubjectCourses
            .SingleAsync(x => x.Id.Equals(subjectCourse.Id));

        // Assert
        int studentGroupAssignmentCount = subjectCourse.Assignments
            .SelectMany(x => x.GroupAssignments)
            .Count(x => x.StudentGroupId.Equals(studentGroup.Id));

        studentGroupAssignmentCount.Should().Be(subjectCourse.Assignments.Count);
    }
}