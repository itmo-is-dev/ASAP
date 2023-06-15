using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Users;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Users;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class UpdateUserNameTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public UpdateUserNameTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateUserNameCorrectly()
    {
        // Arrange
        UserModel user = await _database.Context.Users
            .OrderBy(x => x.Id)
            .FirstAsync();

        string newFirstName = user.FirstName + "_new";
        string newMiddleName = user.MiddleName + "_new";
        string newLastName = user.LastName + "_new";

        var command = new UpdateUserName.Command(user.Id, newFirstName, newMiddleName, newLastName);
        var handler = new UpdateUserNameHandler(_database.PersistenceContext);

        // Act
        await handler.Handle(command, default);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.MiddleName.Should().Be(newMiddleName);
        user.LastName.Should().Be(newLastName);
    }
}