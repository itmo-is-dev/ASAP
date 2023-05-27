using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.Study.SubjectCourses;

#pragma warning disable CA1506
[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CalculatePointsOfTransferredStudentTest : TestBase, IAsyncDisposeLifetime
{
    private readonly CoreDatabaseFixture _database;

    public CalculatePointsOfTransferredStudentTest(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task TransferStudent_CalculateSubmissions_Should_BeEqual()
    {
        Assignment assignment = await _database.Context.Assignments
            .Select(x => x)
            .ToAsyncEnumerable()
            .Where(x =>
                x.GroupAssignments.Count > 1
                && x.GroupAssignments.Any(xx => xx.Submissions.Any(xxx => xxx.IsRated)))
            .FirstAsync();

        Student student = assignment.GroupAssignments
            .SelectMany(x => x.Submissions)
            .First(x => x.IsRated)
            .Student;

        StudentGroup newGroup = assignment.GroupAssignments
            .Select(x => x.Group)
            .First(xx => xx.Students.Contains(student) is false);

        var githubUserService = new Mock<IGithubUserService>();
        githubUserService.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new GithubUserDto(id, "amogus"));

        var subjectCourseService = new SubjectCourseService(
            _database.Context,
            new UserFullNameFormatter(),
            githubUserService.Object);

        SubjectCoursePointsDto pointsDto = await subjectCourseService
            .CalculatePointsAsync(assignment.SubjectCourse.Id, CancellationToken.None);

        int ratedSubmissionCountBefore = pointsDto.StudentsPoints
            .First(x => x.Student.User.Id == student.UserId)
            .Points.Count;

        student.TransferToAnotherGroup(newGroup);

        pointsDto = await subjectCourseService
            .CalculatePointsAsync(assignment.SubjectCourse.Id, CancellationToken.None);

        int ratedSubmissionCountAfter = pointsDto.StudentsPoints
            .First(x => x.Student.User.Id == student.UserId)
            .Points.Count;

        Assert.Equal(ratedSubmissionCountBefore, ratedSubmissionCountAfter);
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}