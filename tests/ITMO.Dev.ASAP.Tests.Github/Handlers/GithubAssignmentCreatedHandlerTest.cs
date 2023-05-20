using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Handlers.Assignments;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Tests.Github.Fixtures;
using ITMO.Dev.ASAP.Tests.Github.Tools;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Github.Handlers;

[Collection(nameof(DatabaseCollectionFixture))]
public class GithubAssignmentCreatedHandlerTest
{
    private readonly Mock<IPersistenceContext> _persistenceContext = new Mock<IPersistenceContext>();
    private readonly Mock<ILogger<AssignmentCreatedHandler>> _logger = new Mock<ILogger<AssignmentCreatedHandler>>();
    private readonly DeterministicFaker _faker;

    public GithubAssignmentCreatedHandlerTest(DeterministicFaker faker)
    {
        _faker = faker;
    }

    [Fact]
    public async void AddDuplicateAssignment_ShouldUpdate()
    {
        var assignmentDto = new AssignmentDto(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
            "amogus",
            "amogus",
            0,
            0,
            5);

        var notification = new AssignmentCreated.Notification(assignmentDto);
        var githubAssignment = new GithubAssignment(Guid.NewGuid(), Guid.NewGuid(), assignmentDto.ShortName);

        _persistenceContext
            .Setup(context => context.Assignments.QueryAsync(It.IsAny<GithubAssignmentQuery>(), CancellationToken.None))
            .Returns(() => new List<GithubAssignment> { githubAssignment }.ToAsyncEnumerable());

        _persistenceContext
            .Setup(context => context.Assignments.Update(It.IsAny<GithubAssignment>()))
            .Verifiable();

        var handler = new AssignmentCreatedHandler(_persistenceContext.Object, _logger.Object);
        await handler.Handle(notification, CancellationToken.None);

        Mock.VerifyAll();
    }
}