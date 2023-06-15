using FluentAssertions;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Subjects;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetSubjectTest : CoreDatabaseTestBase
{
    public GetSubjectTest(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task Handle_ShouldNotEmpty_WhenIssuedByAdmin()
    {
        Guid mentorId = await Context.Mentors
            .Select(x => x.UserId)
            .FirstAsync();

        var adminUser = new AdminUser(mentorId);

        var query = new GetSubjects.Query();
        var handler = new GetSubjectsHandler(PersistenceContext, adminUser);

        GetSubjects.Response response = await handler.Handle(query, CancellationToken.None);

        response.Subjects.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenIssuedByAnonymous()
    {
        var anonymousUser = new AnonymousUser();

        var query = new GetSubjects.Query();
        var handler = new GetSubjectsHandler(PersistenceContext, anonymousUser);

        await Assert.ThrowsAsync<UserHasNotAccessException>(() => handler.Handle(query, default));
    }
}