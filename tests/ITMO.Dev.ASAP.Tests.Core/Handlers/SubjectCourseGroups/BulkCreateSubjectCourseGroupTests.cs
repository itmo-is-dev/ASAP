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
public class BulkCreateSubjectCourseGroupTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public BulkCreateSubjectCourseGroupTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateMultipleGroups()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Where(x => x.Assignments.Count != 0)
            .FirstAsync();

        StudentGroupModel[] studentGroups = Enumerable.Range(0, _database.Faker.Random.Int(10, 15))
            .Select(_ => new StudentGroupModel(_database.Faker.Random.Guid(), _database.Faker.Commerce.ProductName()))
            .ToArray();

        _database.Context.StudentGroups.AddRange(studentGroups);
        await _database.Context.SaveChangesAsync();

        Guid[] studentGroupIds = studentGroups.Select(x => x.Id).ToArray();

        var publisher = new Mock<IPublisher>();

        var command = new BulkCreateSubjectCourseGroups.Command(subjectCourse.Id, studentGroupIds);
        var handler = new BulkCreateSubjectCourseGroupsHandler(_database.PersistenceContext, publisher.Object);

        // Act
        await handler.Handle(command, default);

        subjectCourse = await _database.Context.SubjectCourses
            .SingleAsync(x => x.Id.Equals(subjectCourse.Id));

        // Assert
        foreach (StudentGroupModel studentGroup in studentGroups)
        {
            int studentGroupAssignmentCount = subjectCourse.Assignments
                .SelectMany(x => x.GroupAssignments)
                .Count(x => x.StudentGroupId.Equals(studentGroup.Id));

            studentGroupAssignmentCount.Should().Be(subjectCourse.Assignments.Count);
        }
    }
}