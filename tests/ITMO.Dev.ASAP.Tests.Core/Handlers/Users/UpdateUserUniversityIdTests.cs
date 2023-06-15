using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Users;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Users;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class UpdateUserUniversityIdTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public UpdateUserUniversityIdTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateUniversityId_WhenAssociationExists()
    {
        // Arrange
        UserModel user = await _database.Context.UserAssociations
            .OfType<IsuUserAssociationModel>()
            .Select(x => x.User)
            .OrderBy(x => x.Id)
            .FirstAsync();

        int newUniversityId = int.MaxValue;

        var command = new UpdateUserUniversityId.Command(user.Id, newUniversityId);
        var handler = new UpdateUserUniversityIdHandler(_database.PersistenceContext, new AdminUser(Guid.Empty));

        // Act
        await handler.Handle(command, default);

        // Assert
        user.Associations.Should().ContainSingle()
            .Which.Should().BeOfType<IsuUserAssociationModel>()
            .Which.UniversityId.Should().Be(newUniversityId);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateAssociation_WhenAssociationDoesNotExist()
    {
        // Arrange
        UserModel user = await _database.Context.Users
            .OrderBy(x => x.Id)
            .Where(x => x.Associations.Count == 0)
            .FirstAsync();

        int universityId = int.MaxValue - 1;

        var command = new UpdateUserUniversityId.Command(user.Id, universityId);
        var handler = new UpdateUserUniversityIdHandler(_database.PersistenceContext, new AdminUser(Guid.Empty));

        // Act
        await handler.Handle(command, default);

        // Assert
        user.Associations.Should().ContainSingle()
            .Which.Should().BeOfType<IsuUserAssociationModel>()
            .Which.UniversityId.Should().Be(universityId);
    }
}