using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Subjects;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CreateSubjectTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public CreateSubjectTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateSubject()
    {
        // Arrange
        var command = new CreateSubject.Command(_database.Faker.Commerce.ProductName());
        var handler = new CreateSubjectHandler(_database.PersistenceContext);

        // Act
        CreateSubject.Response response = await handler.Handle(command, default);

        // Assert
        int subjectCount = await _database.Context.Subjects
            .Where(x => x.Id.Equals(response.Subject.Id))
            .CountAsync();

        subjectCount.Should().Be(1);
    }
}