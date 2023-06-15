using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Submissions;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class ActivateSubmissionTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public ActivateSubmissionTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldSetSubmissionStateActive()
    {
        // Arrange
        SubmissionModel submission = await _database.Context.Submissions
            .OrderBy(x => x.Id)
            .Where(x => x.State == SubmissionStateKind.Inactive)
            .FirstAsync();

        var command = new ActivateSubmission.Command(submission.Id);
        var handler = new ActivateSubmissionHandler(_database.PersistenceContext, Mock.Of<IPublisher>());

        // Act
        await handler.Handle(command, default);

        // Assert
        submission.State.Should().Be(SubmissionStateKind.Active);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishUpdatedSubmissionWithoutCancellation()
    {
        // Arrange
        SubmissionModel submission = await _database.Context.Submissions
            .OrderBy(x => x.Id)
            .Where(x => x.State == SubmissionStateKind.Inactive)
            .FirstAsync();

        var publisher = new Mock<IPublisher>();

        var command = new ActivateSubmission.Command(submission.Id);
        var handler = new ActivateSubmissionHandler(_database.PersistenceContext, publisher.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromHours(10));

        // Act
        await handler.Handle(command, cts.Token);

        // Assert
        publisher.Verify(
            x => x.Publish(It.IsAny<SubmissionStateUpdated.Notification>(), default),
            Times.Once);
    }
}