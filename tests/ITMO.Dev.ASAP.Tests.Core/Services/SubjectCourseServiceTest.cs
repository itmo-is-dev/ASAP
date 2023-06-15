using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.SubjectCourses;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Services;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class SubjectCourseServiceTest : CoreDatabaseTestBase
{
    public SubjectCourseServiceTest(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task CalculatePointsAsync_Should_ReturnPoints()
    {
        // Arrange
        SubjectCourseModel course = await Context.SubjectCourses
            .Where(x => x.Assignments
                .SelectMany(xx => xx.GroupAssignments)
                .SelectMany(xx => xx.Submissions)
                .Any())
            .FirstAsync();

        SubjectCourseService service = CreateService();

        // Act
        SubjectCoursePointsDto points = await service.CalculatePointsAsync(course.Id, default);

        // Assert
        points.StudentsPoints.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CalculatePointsAsync_Should_ReturnUniqueAssignmentIds()
    {
        // Arrange
        SubjectCourseModel course = await Context.SubjectCourses
            .Where(x => x.Assignments
                .SelectMany(xx => xx.GroupAssignments)
                .SelectMany(xx => xx.Submissions)
                .Any())
            .FirstAsync();

        SubjectCourseService service = CreateService();

        // Act
        SubjectCoursePointsDto points = await service.CalculatePointsAsync(course.Id, default);

        // Assert
        IReadOnlyList<StudentPointsDto> studentPoints = points.StudentsPoints;

        int uniqueStudentPoints = studentPoints
            .Select(x => x.Points)
            .Count(x => x.Select(xx => xx.AssignmentId).Distinct().Count() == x.Count);

        Assert.Equal(uniqueStudentPoints, studentPoints.Count);
    }

    public SubjectCourseService CreateService()
    {
        var githubUserService = new Mock<IGithubUserService>();

        githubUserService
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new GithubUserDto(id, id.ToString()));

        return new SubjectCourseService(
            PersistenceContext,
            new UserFullNameFormatter(),
            githubUserService.Object);
    }
}