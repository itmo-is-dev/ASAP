using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourses;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateSubjectCourseTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public CreateSubjectCourseTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateSubjectCourse()
    {
        // Arrange
        SubjectModel subject = await _database.Context.Subjects
            .OrderBy(x => x.Id)
            .FirstAsync();

        var command = new CreateSubjectCourse.Command(
            subject.Id,
            _database.Faker.Commerce.ProductName(),
            SubmissionStateWorkflowTypeDto.ReviewWithDefense);

        var handler = new CreateSubjectCourseHandler(_database.PersistenceContext, Mock.Of<IPublisher>());

        // Act
        CreateSubjectCourse.Response response = await handler.Handle(command, default);

        // Assert
        int subjectCourseCount = await _database.Context.SubjectCourses
            .Where(x => x.Id.Equals(response.SubjectCourse.Id))
            .CountAsync();

        subjectCourseCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishSubjectCourseWithoutCancellation()
    {
        // Arrange
        SubjectModel subject = await _database.Context.Subjects
            .OrderBy(x => x.Id)
            .FirstAsync();

        var publisher = new Mock<IPublisher>();

        var command = new CreateSubjectCourse.Command(
            subject.Id,
            _database.Faker.Commerce.ProductName(),
            SubmissionStateWorkflowTypeDto.ReviewWithDefense);

        var handler = new CreateSubjectCourseHandler(_database.PersistenceContext, publisher.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromHours(10));

        // Act
        await handler.Handle(command, cts.Token);

        // Assert
        publisher.Verify(
            x => x.Publish(It.IsAny<SubjectCourseCreated.Notification>(), default),
            Times.Once);
    }
}