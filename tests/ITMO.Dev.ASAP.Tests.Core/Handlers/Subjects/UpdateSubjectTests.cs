using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Subjects;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class UpdateSubjectTests : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public UpdateSubjectTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateSubject()
    {
        // Arrange
        SubjectModel subject = await _database.Context.Subjects
            .OrderBy(x => x.Id)
            .FirstAsync();

        string newTitle = subject.Title + "_new";

        var command = new UpdateSubject.Command(subject.Id, newTitle);
        var handler = new UpdateSubjectHandler(_database.PersistenceContext);

        // Act
        await handler.Handle(command, default);

        // Assert
        subject.Title.Should().Be(newTitle);
    }
}