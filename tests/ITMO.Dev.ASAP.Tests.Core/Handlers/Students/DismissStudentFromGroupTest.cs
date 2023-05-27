using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Students;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class DismissStudentFromGroupTest : TestBase, IAsyncDisposeLifetime
{
    private readonly CoreDatabaseFixture _database;

    public DismissStudentFromGroupTest(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Handle_Should_NotThrow()
    {
        Guid studentId = await _database.Context.Students
            .Where(x => x.Group != null)
            .Select(x => x.UserId)
            .FirstAsync();

        var command = new DismissStudentFromGroup.Command(studentId);
        var handler = new DismissStudentFromGroupHandler(_database.Context);

        await handler.Handle(command, default);
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}