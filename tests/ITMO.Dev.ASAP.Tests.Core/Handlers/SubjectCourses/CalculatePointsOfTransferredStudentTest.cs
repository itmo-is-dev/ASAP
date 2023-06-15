using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Application.SubjectCourses;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourses;

#pragma warning disable CA1506
[Collection(nameof(CoreDatabaseCollectionFixture))]
public class CalculatePointsOfTransferredStudentTest : CoreDatabaseTestBase
{
    public CalculatePointsOfTransferredStudentTest(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task TransferStudent_CalculateSubmissions_Should_BeEqual()
    {
        AssignmentModel assignmentModel = await Context.Assignments
            .Select(x => x)
            .ToAsyncEnumerable()
            .Where(x =>
                x.GroupAssignments.Count > 1
                && x.GroupAssignments.Any(xx => xx.Submissions.Any(xxx => xxx.Rating != null)))
            .FirstAsync();

        StudentModel studentModel = assignmentModel.GroupAssignments
            .SelectMany(x => x.Submissions)
            .First(x => x.Rating != null)
            .Student;

        StudentGroupModel newGroupModel = assignmentModel.GroupAssignments
            .Select(x => x.StudentGroup)
            .First(xx => xx.Students.Contains(studentModel) is false);

        var githubUserService = new Mock<IGithubUserService>();
        githubUserService.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new GithubUserDto(id, "amogus"));

        var subjectCourseService = new SubjectCourseService(
            PersistenceContext,
            new UserFullNameFormatter(),
            githubUserService.Object);

        SubjectCoursePointsDto pointsDto = await subjectCourseService
            .CalculatePointsAsync(assignmentModel.SubjectCourse.Id, CancellationToken.None);

        int ratedSubmissionCountBefore = pointsDto.StudentsPoints
            .First(x => x.Student.User.Id == studentModel.UserId)
            .Points.Count;

        Student student = await PersistenceContext.Students
            .GetByIdAsync(studentModel.UserId, default);

        StudentGroup? oldGroup = student.Group is null
            ? null
            : await PersistenceContext.StudentGroups.GetByIdAsync(student.Group.Id, default);

        StudentGroup newGroup = await PersistenceContext.StudentGroups
            .GetByIdAsync(newGroupModel.Id, default);

        student.TransferToAnotherGroup(oldGroup, newGroup);

        pointsDto = await subjectCourseService
            .CalculatePointsAsync(assignmentModel.SubjectCourse.Id, CancellationToken.None);

        int ratedSubmissionCountAfter = pointsDto.StudentsPoints
            .First(x => x.Student.User.Id == student.UserId)
            .Points.Count;

        Assert.Equal(ratedSubmissionCountBefore, ratedSubmissionCountAfter);
    }
}