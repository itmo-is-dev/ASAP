using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;
using ITMO.Dev.ASAP.Application.Users;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.Study.Subjects;

public class GetSubjectTest : TestBase
{
    [Fact]
    public async Task Handle_By_Admin_Should_NotEmpty()
    {
        Guid mentorId = await Context.Mentors
            .Select(x => x.UserId)
            .FirstAsync();

        var adminUser = new AdminUser(mentorId);

        var handler = new GetSubjectsHandler(Context, adminUser);
        var query = new GetSubjects.Query();
        GetSubjects.Response response = await handler.Handle(query, CancellationToken.None);

        Assert.NotEmpty(response.Subjects);
    }

    [Fact]
    public async Task Handle_By_Anonymous_ShouldThrow()
    {
        var anonymousUser = new AnonymousUser();

        var handler = new GetSubjectsHandler(Context, anonymousUser);
        var query = new GetSubjects.Query();

        await Assert.ThrowsAsync<UserHasNotAccessException>(
            () => handler.Handle(query, CancellationToken.None));
    }
}