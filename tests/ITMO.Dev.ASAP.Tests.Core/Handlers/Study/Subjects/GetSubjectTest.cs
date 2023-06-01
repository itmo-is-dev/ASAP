using FluentAssertions;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Study.Subjects;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetSubjectTest : TestBase, IAsyncDisposeLifetime
{
    private readonly CoreDatabaseFixture _database;

    public GetSubjectTest(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Handle_By_Admin_Should_NotEmpty()
    {
        Guid mentorId = await _database.Context.Mentors
            .Select(x => x.UserId)
            .FirstAsync();

        var adminUser = new AdminUser(mentorId);

        var query = new GetSubjects.Query();
        var handler = new GetSubjectsHandler(_database.Context, adminUser);

        GetSubjects.Response response = await handler.Handle(query, CancellationToken.None);

        response.Subjects.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_By_Anonymous_ShouldThrow()
    {
        var anonymousUser = new AnonymousUser();

        var query = new GetSubjects.Query();
        var handler = new GetSubjectsHandler(_database.Context, anonymousUser);

        await Assert.ThrowsAsync<UserHasNotAccessException>(() => handler.Handle(query, default));
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}