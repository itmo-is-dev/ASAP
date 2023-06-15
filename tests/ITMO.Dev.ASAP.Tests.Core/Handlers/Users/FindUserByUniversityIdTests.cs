using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Handlers.Users;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Users;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class FindUserByUniversityIdTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public FindUserByUniversityIdTests(CoreDatabaseFixture database, ITestOutputHelper output) : base(output)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectUser_WhenUniversityIdExists()
    {
        // Arrange
        var user = await _database.Context.UserAssociations
            .OfType<IsuUserAssociationModel>()
            .Select(x => new { x.UserId, x.UniversityId })
            .OrderBy(x => x.UserId)
            .FirstAsync();

        var query = new FindUserByUniversityId.Query(user.UniversityId);
        var handler = new FindUserByUniversityIdHandler(_database.PersistenceContext);

        // Act
        FindUserByUniversityId.Response response = await handler.Handle(query, default);

        // Assert
        response.User.Should().NotBeNull().And.Subject.As<UserDto>().Id.Should().Be(user.UserId);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenUniversityIdDoesNotExist()
    {
        // Arrange
        var query = new FindUserByUniversityId.Query(int.MinValue);
        var handler = new FindUserByUniversityIdHandler(_database.PersistenceContext);

        // Act
        FindUserByUniversityId.Response response = await handler.Handle(query, default);

        // Assert
        response.User.Should().BeNull();
    }
}