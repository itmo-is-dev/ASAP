using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.StudyGroups;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class BulkGetStudyGroupsTests : CoreDatabaseTestBase
{
    public BulkGetStudyGroupsTests(CoreDatabaseFixture fixture) : base(fixture) { }

    [Fact]
    public async Task HandleAsync_ShouldReturnGroups()
    {
        // Arrange
        List<Guid> ids = await Context.StudentGroups
            .OrderBy(x => x.Id)
            .Take(Fixture.Faker.Random.Int(10, 20))
            .Select(x => x.Id)
            .ToListAsync();

        var query = new BulkGetStudyGroups.Query(ids);
        var handler = new BulkGetStudentGroupsHandler(PersistenceContext);

        // Act
        BulkGetStudyGroups.Response response = await handler.Handle(query, default);

        // Assert
        response.Groups.Count.Should().Be(ids.Count);
    }
}