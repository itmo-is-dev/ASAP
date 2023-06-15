using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourses;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateAssignmentTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public CreateAssignmentTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateAssignmentCorrectly()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses
            .OrderBy(x => x.Id)
            .FirstAsync();

        var publisher = new Mock<IPublisher>();

        var command = new CreateAssignment.Command(
            subjectCourse.Id,
            _database.Faker.Commerce.ProductName(),
            _database.Faker.Commerce.ProductName(),
            _database.Faker.Random.Int(10, 20),
            _database.Faker.Random.Double(0, 10),
            _database.Faker.Random.Double(10, 20));

        var handler = new CreateAssignmentHandler(_database.PersistenceContext, publisher.Object);

        // Act
        CreateAssignment.Response response = await handler.Handle(command, default);

        // Assert
        int assignmentExists = await _database.Context.Assignments
            .CountAsync(x => x.Id.Equals(response.Assignment.Id));

        assignmentExists.Should().Be(1);
    }
}