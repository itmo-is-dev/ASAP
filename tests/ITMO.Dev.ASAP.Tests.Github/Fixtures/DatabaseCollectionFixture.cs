using ITMO.Dev.ASAP.Tests.Github.Tools;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Github.Fixtures;

[CollectionDefinition(nameof(DatabaseCollectionFixture))]
public class DatabaseCollectionFixture : ICollectionFixture<GithubDatabaseFixture>, ICollectionFixture<DeterministicFaker> { }