using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Submissions;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class BanSubmissionTests : CoreDatabaseTestBase
{
    public BanSubmissionTests(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task HandleAsync_ShouldSetSubmissionStateBanned_WhenIssuedByMentor()
    {
        // Arrange
        SubmissionModel submission = await Context.Submissions
            .OrderBy(x => x.Id)
            .Where(x => x.State == SubmissionStateKind.Active)
            .FirstAsync();

        Guid issuerId = submission.GroupAssignment.Assignment.SubjectCourse.Mentors.First().UserId;

        var command = new BanSubmission.Command(
            issuerId,
            submission.StudentId,
            submission.AssignmentId,
            submission.Code);

        var handler = new BanSubmissionHandler(
            GetRequiredService<IPermissionValidator>(),
            PersistenceContext,
            Mock.Of<IPublisher>());

        // Act
        await handler.Handle(command, default);

        // Assert
        Context.ChangeTracker.Clear();
        submission = await Context.Submissions.SingleAsync(x => x.Id.Equals(submission.Id));

        submission.State.Should().Be(SubmissionStateKind.Banned);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenIssuedNotByMentor()
    {
        // Arrange
        SubmissionModel submission = await Context.Submissions
            .OrderBy(x => x.Id)
            .Where(x => x.State == SubmissionStateKind.Active)
            .FirstAsync();

        IEnumerable<Guid> mentorIds = submission.GroupAssignment.Assignment.SubjectCourse.Mentors.Select(x => x.UserId);

        UserModel user = await Context.Users
            .OrderBy(x => x.Id)
            .Where(u => mentorIds.Contains(u.Id) == false)
            .FirstAsync();

        var command = new BanSubmission.Command(
            user.Id,
            submission.StudentId,
            submission.AssignmentId,
            submission.Code);

        var handler = new BanSubmissionHandler(
            GetRequiredService<IPermissionValidator>(),
            PersistenceContext,
            Mock.Of<IPublisher>());

        // Act
        Func<Task<BanSubmission.Response>> action = () => handler.Handle(command, default);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishUpdatedSubmissionWithoutCancellation()
    {
        // Arrange
        SubmissionModel submission = await Context.Submissions
            .OrderBy(x => x.Id)
            .Where(x => x.State == SubmissionStateKind.Active)
            .FirstAsync();

        var publisher = new Mock<IPublisher>();

        var command = new BanSubmission.Command(
            Guid.Empty,
            submission.StudentId,
            submission.AssignmentId,
            submission.Code);

        var handler = new BanSubmissionHandler(
            Mock.Of<IPermissionValidator>(),
            PersistenceContext,
            publisher.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromHours(10));

        // Act
        await handler.Handle(command, cts.Token);

        // Assert
        publisher.Verify(
            x => x.Publish(It.IsAny<SubmissionUpdated.Notification>(), default),
            Times.Once);
    }
}