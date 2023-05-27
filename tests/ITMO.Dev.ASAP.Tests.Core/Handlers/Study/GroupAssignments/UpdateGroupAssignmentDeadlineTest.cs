using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.GroupAssignments;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using ITMO.Dev.ASAP.Seeding.Options;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Study.GroupAssignments;

public class UpdateGroupAssignmentDeadlineTest :
    TestBase,
    IClassFixture<UpdateGroupAssignmentDeadlineTest.UpdateGroupAssignmentDatabaseFixture>,
    IAsyncDisposeLifetime
{
    private readonly DateOnly _newDeadline = DateOnly.MaxValue;
    private readonly IPublisher _publisher = new Mock<IPublisher>().Object;
    private readonly GroupAssignment _groupAssignment;
    private readonly UpdateGroupAssignmentDatabaseFixture _database;

    public UpdateGroupAssignmentDeadlineTest(UpdateGroupAssignmentDatabaseFixture database)
    {
        _database = database;
        _groupAssignment = database.Context.GroupAssignments.First();
    }

    [Fact]
    public async Task Handle_ByMentorOfThisCourse_ShouldUpdateDeadline()
    {
        Mentor mentor = await _database.Context.Mentors
            .FirstAsync(m => m.Course.Equals(_groupAssignment.Assignment.SubjectCourse));

        var currentUser = new MentorUser(mentor.UserId);
        UpdateGroupAssignmentDeadline.Response response = await HandleByCurrentUser(currentUser);
        Assert.Equal(_newDeadline.AsDateTime(), response.GroupAssignment.Deadline);
    }

    [Fact]
    public async Task Handle_ByMentorOfNotThisCourse_ShouldThrow()
    {
        Mentor mentor = await _database.Context.Mentors
            .FirstAsync(m => !m.Course.Equals(_groupAssignment.Assignment.SubjectCourse));

        var currentUser = new MentorUser(mentor.UserId);
        await Assert.ThrowsAsync<AccessDeniedException>(() => HandleByCurrentUser(currentUser));
    }

    [Fact]
    public async Task Handle_ByModerator_ShouldUpdateDeadline()
    {
        var currentUser = new ModeratorUser(Guid.NewGuid());
        UpdateGroupAssignmentDeadline.Response response = await HandleByCurrentUser(currentUser);
        Assert.Equal(_newDeadline.AsDateTime(), response.GroupAssignment.Deadline);
    }

    [Fact]
    public async Task Handle_ByAdmin_ShouldUpdateDeadline()
    {
        var currentUser = new AdminUser(Guid.NewGuid());
        UpdateGroupAssignmentDeadline.Response response = await HandleByCurrentUser(currentUser);
        Assert.Equal(_newDeadline.AsDateTime(), response.GroupAssignment.Deadline);
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }

    private async Task<UpdateGroupAssignmentDeadline.Response> HandleByCurrentUser(ICurrentUser currentUser)
    {
        var handler = new UpdateGroupAssignmentDeadlineHandler(_database.Context, _publisher, currentUser);

        var command = new UpdateGroupAssignmentDeadline.Command(
            _groupAssignment.GroupId,
            _groupAssignment.AssignmentId,
            _newDeadline);

        return await handler.Handle(command, default);
    }

    // Custom fixture for custom seeding config
    public class UpdateGroupAssignmentDatabaseFixture : CoreDatabaseFixture
    {
        protected override void ConfigureSeeding(EntityGenerationOptions options)
        {
            options.ConfigureEntityGenerator<SubjectCourse>(x => x.Count = 2);
        }
    }
}