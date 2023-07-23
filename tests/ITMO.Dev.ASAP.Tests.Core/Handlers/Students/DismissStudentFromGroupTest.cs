using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Students;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class DismissStudentFromGroupTest : CoreDatabaseTestBase, IAsyncDisposeLifetime
{
    public DismissStudentFromGroupTest(CoreDatabaseFixture database, ITestOutputHelper output)
        : base(database, output) { }

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

    [Fact]
    public async Task HandleAsync_ShouldSetStudentIdNull()
    {
        // Arrange
        Guid studentId = await Context.Students
            .Where(x => x.StudentGroup != null)
            .Select(x => x.UserId)
            .FirstAsync();

        var command = new DismissStudentFromGroup.Command(studentId);
        var handler = new DismissStudentFromGroupHandler(PersistenceContext);

        // Act
        await handler.Handle(command, default);

        // Assert
        Context.ChangeTracker.Clear();

        StudentModel student = await Context.Students
            .SingleAsync(x => x.UserId.Equals(studentId));

        student.StudentGroupId.Should().BeNull();
    }
}