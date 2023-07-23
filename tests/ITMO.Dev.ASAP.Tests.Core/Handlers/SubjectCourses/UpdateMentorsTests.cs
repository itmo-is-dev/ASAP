using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourses;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class UpdateMentorsTests : CoreDatabaseTestBase
{
    public UpdateMentorsTests(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task HandleAsync_ShouldUpdateMentorsCorrectly()
    {
        // Arrange
        SubjectCourseModel subjectCourse = await Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Where(x => x.Mentors.Count != 0)
            .FirstAsync();

        UserModel user = await Context.Users
            .OrderBy(x => x.Id)
            .Where(u => subjectCourse.Mentors.Select(x => x.UserId).Contains(u.Id) == false)
            .FirstAsync();

        MentorModel removedMentor = subjectCourse.Mentors.First();

        Guid[] userIds = subjectCourse.Mentors
            .Select(x => x.UserId)
            .Skip(1)
            .Append(user.Id)
            .ToArray();

        var command = new UpdateMentors.Command(subjectCourse.Id, userIds);
        var handler = new UpdateMentorsHandler(PersistenceContext);

        // Act
        await handler.Handle(command, default);

        // Assert
        Context.ChangeTracker.Clear();
        subjectCourse = await Context.SubjectCourses.SingleAsync(x => x.Id.Equals(subjectCourse.Id));

        subjectCourse.Mentors.Should().NotContain(x => x.UserId.Equals(removedMentor.UserId));
        subjectCourse.Mentors.Should().Contain(x => x.UserId.Equals(user.Id));
    }
}