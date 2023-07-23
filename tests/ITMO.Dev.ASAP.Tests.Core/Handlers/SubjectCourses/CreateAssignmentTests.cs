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
public class CreateAssignmentTests : CoreDatabaseTestBase
{
    public CreateAssignmentTests(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task HandleAsync_ShouldCreateAssignmentCorrectly()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await Context.SubjectCourses
            .OrderBy(x => x.Id)
            .FirstAsync();

        var publisher = new Mock<IPublisher>();

        var command = new CreateAssignment.Command(
            subjectCourse.Id,
            Fixture.Faker.Commerce.ProductName(),
            Fixture.Faker.Commerce.ProductName(),
            Fixture.Faker.Random.Int(10, 20),
            Fixture.Faker.Random.Double(0, 10),
            Fixture.Faker.Random.Double(10, 20));

        var handler = new CreateAssignmentHandler(PersistenceContext, publisher.Object);

        // Act
        CreateAssignment.Response response = await handler.Handle(command, default);

        // Assert
        int assignmentExists = await Context.Assignments
            .CountAsync(x => x.Id.Equals(response.Assignment.Id));

        assignmentExists.Should().Be(1);
    }
}