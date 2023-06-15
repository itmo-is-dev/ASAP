using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Students;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class DismissStudentFromGroupTest : CoreDatabaseTestBase, IAsyncDisposeLifetime
{
    public DismissStudentFromGroupTest(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task Handle_Should_NotThrow()
    {
        // Arrange
        Guid studentId = await Context.Students
            .Where(x => x.StudentGroup != null)
            .Select(x => x.UserId)
            .FirstAsync();

        var command = new DismissStudentFromGroup.Command(studentId);
        var handler = new DismissStudentFromGroupHandler(PersistenceContext);

        // Act
        Func<Task> action = () => handler.Handle(command, default);

        // Assert
        await action.Should().NotThrowAsync();
    }
}