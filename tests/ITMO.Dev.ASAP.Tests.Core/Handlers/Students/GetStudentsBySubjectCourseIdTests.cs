using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Students;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetStudentsBySubjectCourseIdTests : TestBase, IAsyncDisposeLifetime
{
    private readonly CoreDatabaseFixture _database;

    public GetStudentsBySubjectCourseIdTests(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Handle_Should_NoThrow()
    {
        SubjectCourse subjectCourse = _database.Context.SubjectCourses
            .First(sc => sc.Groups.Any(g => g.StudentGroup.Students.Any()));

        var githubUserService = new Mock<IGithubUserService>();

        githubUserService
            .Setup(x => x.FindByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GithubUserDto>());

        var query = new GetStudentsBySubjectCourseId.Query(subjectCourse.Id);
        var handler = new GetStudentsBySubjectCourseIdHandler(_database.Context, githubUserService.Object);

        GetStudentsBySubjectCourseId.Response handle = await handler.Handle(query, CancellationToken.None);

        handle.Students.Should().NotBeEmpty();
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}