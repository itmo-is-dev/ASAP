using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.StudyGroups;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class TransferStudentTest : CoreTestBase
{
    private readonly CoreDatabaseFixture _database;

    public TransferStudentTest(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HandleAsync_ShouldTransferStudentCorrectly()
    {
        // Arrange
        List<StudentGroupModel> groups = await _database.Context.StudentGroups
            .OrderBy(x => x.Id)
            .Where(x => x.Students.Count != 0)
            .Take(2)
            .ToListAsync();

        Guid studentId = groups[0].Students.First().UserId;
        StudentGroupModel group = groups[1];

        var publisher = new Mock<IPublisher>();

        var githubUserServiceMock = new Mock<IGithubUserService>();

        githubUserServiceMock
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, CancellationToken _) => null);

        var command = new TransferStudent.Command(studentId, group.Id);

        var handler = new TransferStudentHandler(
            _database.PersistenceContext,
            publisher.Object,
            githubUserServiceMock.Object);

        // Act
        TransferStudent.Response response = await handler.Handle(command, default);

        // Assert
        response.Student.GroupName.Should().Be(group.Name);
    }
}