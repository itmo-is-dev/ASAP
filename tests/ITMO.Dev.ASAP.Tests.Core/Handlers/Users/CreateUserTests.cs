using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Users;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateUserTests : CoreDatabaseTestBase
{
    public CreateUserTests(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task HandleAsync_ShouldCreateUser()
    {
        // Arrange
        var command = new CreateUser.Command(
            Fixture.Faker.Name.FirstName(),
            Fixture.Faker.Internet.UserName(),
            Fixture.Faker.Name.LastName());

        var handler = new CreateUserHandler(PersistenceContext);

        // Act
        CreateUser.Response response = await handler.Handle(command, default);

        // Assert
        int userCount = await Context.Users
            .Where(x => x.Id.Equals(response.User.Id))
            .CountAsync();

        userCount.Should().Be(1);
    }
}