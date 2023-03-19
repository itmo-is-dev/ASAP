using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.Students;

public class GetStudentsBySubjectCourseIdTests : TestBase
{
    [Fact]
    public async Task Handle_Should_NoThrow()
    {
        SubjectCourse subjectCourse = Context.SubjectCourses
            .First(sc => sc.Groups.Any(g => g.StudentGroup.Students.Any()));

        var githubUserService = new Mock<IGithubUserService>();

        githubUserService
            .Setup(x => x.FindByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GithubUserDto>());

        var query = new GetStudentsBySubjectCourseId.Query(subjectCourse.Id);
        var handler = new GetStudentsBySubjectCourseIdHandler(Context, githubUserService.Object);

        GetStudentsBySubjectCourseId.Response handle = await handler.Handle(query, CancellationToken.None);

        handle.Students.Should().NotBeEmpty();
    }
}