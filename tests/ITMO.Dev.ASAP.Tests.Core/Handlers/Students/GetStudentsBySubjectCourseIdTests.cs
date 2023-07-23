using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourses.SubjectCourse;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Students;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetStudentsBySubjectCourseIdTests : CoreDatabaseTestBase, IAsyncDisposeLifetime
{
    public GetStudentsBySubjectCourseIdTests(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task Handle_Should_NoThrow()
    {
        Guid subjectCourseId = await Context.SubjectCourses
            .Where(sc => sc.SubjectCourseGroups.Any(g => g.StudentGroup.Students.Any()))
            .Select(x => x.Id)
            .FirstAsync();

        SubjectCourse subjectCourse = await PersistenceContext.SubjectCourses
            .GetByIdAsync(subjectCourseId, default);

        var githubUserService = new Mock<IGithubUserService>();

        githubUserService
            .Setup(x => x.FindByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GithubUserDto>());

        var query = new GetStudentsBySubjectCourseId.Query(subjectCourse.Id);
        var handler = new GetStudentsBySubjectCourseIdHandler(PersistenceContext, githubUserService.Object);

        GetStudentsBySubjectCourseId.Response handle = await handler.Handle(query, CancellationToken.None);

        handle.Students.Should().NotBeEmpty();
    }
}