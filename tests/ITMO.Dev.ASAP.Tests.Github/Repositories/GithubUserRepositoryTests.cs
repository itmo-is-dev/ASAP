using Dapper;
using FluentAssertions;
using ITMO.Dev.ASAP.Github.DataAccess;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Tests.Github.Extensions;
using ITMO.Dev.ASAP.Tests.Github.Fixtures;
using ITMO.Dev.ASAP.Tests.Github.Tools;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Github.Repositories;

[Collection(nameof(DatabaseCollectionFixture))]
public class GithubUserRepositoryTests : IAsyncLifetime
{
    private readonly DatabaseFixture _database;
    private readonly DeterministicFaker _faker;

    public GithubUserRepositoryTests(DatabaseFixture database, DeterministicFaker faker)
    {
        _database = database;
        _faker = faker;
    }

    [Fact]
    public async Task AddRange_ShouldAddManyRecords()
    {
        // Arrange
        using var unit = new UnitOfWork(_database.Connection);
        var repository = new GithubUserRepository(_database.Connection, unit);

        GithubUser[] users = Enumerable.Range(0, 2).Select(_ => _faker.GithubUser()).ToArray();

        // Act
        repository.AddRange(users);
        await unit.CommitAsync(default);

        // Assert
        const string sql = """
        select "Id", "Username" from "GithubUsers"
        """;

        IEnumerable<GithubUserModel> userModels = _database.RawConnection.Query<GithubUserModel>(sql);

        userModels.Should().BeEquivalentTo(users, x => x.ComparingByMembers<GithubUser>());
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}