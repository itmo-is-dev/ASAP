using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Services;

public class SubjectCourseServiceTest : TestBase
{
    private readonly ISubjectCourseService _service;

    public SubjectCourseServiceTest() : base()
    {
        var githubUserService = new Mock<IGithubUserService>();

        githubUserService
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new GithubUserDto(id, id.ToString()));

        _service = new SubjectCourseService(Context, new UserFullNameFormatter(), githubUserService.Object);
    }

    [Fact]
    public async Task CalculatePointsAsync_Should_ReturnPoints()
    {
        SubjectCourse course = await Context.SubjectCourses
            .Where(x => x.Assignments
                .SelectMany(xx => xx.GroupAssignments)
                .SelectMany(xx => xx.Submissions)
                .Any())
            .FirstAsync();

        SubjectCoursePointsDto points = await _service.CalculatePointsAsync(course.Id, default);

        points.StudentsPoints.Should().NotBeEmpty();
    }
}