using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Users;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateUserTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public CreateUserTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateUser()
    {
        // Arrange
        var command = new CreateUser.Command(
            _database.Faker.Name.FirstName(),
            _database.Faker.Internet.UserName(),
            _database.Faker.Name.LastName());

        var handler = new CreateUserHandler(_database.PersistenceContext);

        // Act
        CreateUser.Response response = await handler.Handle(command, default);

        // Assert
        int userCount = await _database.Context.Users
            .Where(x => x.Id.Equals(response.User.Id))
            .CountAsync();

        userCount.Should().Be(1);
    }
}