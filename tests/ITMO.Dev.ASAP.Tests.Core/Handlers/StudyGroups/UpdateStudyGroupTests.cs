using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.StudyGroups;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class UpdateStudyGroupTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public UpdateStudyGroupTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateGroupCorrectly()
    {
        // Arrange
        Guid groupId = await _database.Context.StudentGroups
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .FirstAsync();

        string name = _database.Faker.Commerce.ProductName();

        var publisher = new Mock<IPublisher>();

        var command = new UpdateStudyGroup.Command(groupId, name);
        var handler = new UpdateStudyGroupHandler(_database.PersistenceContext, publisher.Object);

        // Act
        UpdateStudyGroup.Response response = await handler.Handle(command, default);

        // Assert
        response.Group.Name.Should().Be(name);
    }
}