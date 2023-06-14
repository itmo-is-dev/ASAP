using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;

namespace ITMO.Dev.ASAP.Application.SubjectCourses;

public class SubjectCourseService : ISubjectCourseService
{
    private readonly IPersistenceContext _context;
    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly IGithubUserService _githubUserService;

    public SubjectCourseService(
        IPersistenceContext context,
        IUserFullNameFormatter userFullNameFormatter,
        IGithubUserService githubUserService)
    {
        _context = context;
        _userFullNameFormatter = userFullNameFormatter;
        _githubUserService = githubUserService;
    }

    public async Task<SubjectCoursePointsDto> CalculatePointsAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        StudentAssignment[] studentAssignments = await _context.StudentAssignments
            .GetBySubjectCourseIdAsync(subjectCourseId, cancellationToken)
            .OrderBy(x => x.Assignment.Order)
            .ToArrayAsync(cancellationToken);

        StudentPointsDto[] studentPoints = await studentAssignments
            .GroupBy(x => x.Student)
            .ToAsyncEnumerable()
            .SelectAwait(x => MapToStudentPoints(x, cancellationToken))
            .OrderBy(x => x.Student.GroupName)
            .ThenBy(x => _userFullNameFormatter.GetFullName(x.Student.User))
            .ToArrayAsync(cancellationToken);

        AssignmentDto[] assignments = studentAssignments
            .Select(x => x.Assignment)
            .Distinct()
            .Select(x => x.ToDto(subjectCourseId))
            .ToArray();

        return new SubjectCoursePointsDto(assignments, studentPoints);
    }

    private async ValueTask<StudentPointsDto> MapToStudentPoints(
        IGrouping<Student, StudentAssignment> grouping,
        CancellationToken cancellationToken)
    {
        GithubUserDto? githubUser = await _githubUserService.FindByIdAsync(grouping.Key.UserId, cancellationToken);
        StudentDto studentDto = grouping.Key.ToDto(githubUser?.Username);

        AssignmentPointsDto[] pointsDto = grouping
            .Select(x => x.CalculatePoints())
            .WhereNotNull()
            .Select(x => new AssignmentPointsDto(x.Assignment.Id, x.SubmissionDate, x.IsBanned, x.Points.Value))
            .ToArray();

        return new StudentPointsDto(studentDto, pointsDto);
    }
}