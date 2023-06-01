using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Fixtures;

[CollectionDefinition(nameof(CoreDatabaseCollectionFixture))]
public class CoreDatabaseCollectionFixture : ICollectionFixture<CoreDatabaseFixture> { }