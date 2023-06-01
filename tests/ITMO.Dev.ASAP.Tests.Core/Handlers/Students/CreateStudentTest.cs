using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Students;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateStudentTest : TestBase, IAsyncDisposeLifetime
{
    private readonly CoreDatabaseFixture _database;

    public CreateStudentTest(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Handle_Should_NotThrow()
    {
        Guid groupId = await _database.Context.StudentGroups
            .Select(x => x.Id)
            .FirstAsync();

        var githubUserService = new Mock<IGithubUserService>();

        githubUserService.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new GithubUserDto(id, "123"));

        var command = new CreateStudent.Command("A", "B", "C", groupId);
        var handler = new CreateStudentHandler(_database.Context, githubUserService.Object);

        CreateStudent.Response response = await handler.Handle(command, default);

        response.Student.Should().NotBeNull();
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}