using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Users;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Users;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetUserByIdTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public GetUserByIdTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        UserModel user = await _database.Context.Users
            .OrderBy(x => x.Id)
            .FirstAsync();

        var query = new GetUserById.Query(user.Id);
        var handler = new GetUserByIdHandler(_database.PersistenceContext);

        // Act
        Func<Task<GetUserById.Response>> action = () => handler.Handle(query, default);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenUserNotExist()
    {
        // Arrange
        var query = new GetUserById.Query(Guid.Empty);
        var handler = new GetUserByIdHandler(_database.PersistenceContext);

        // Act
        Func<Task<GetUserById.Response>> action = () => handler.Handle(query, default);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>();
    }
}