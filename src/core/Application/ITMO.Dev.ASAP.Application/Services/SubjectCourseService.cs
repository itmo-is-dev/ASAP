using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;
using Microsoft.EntityFrameworkCore;
using Student = ITMO.Dev.ASAP.Domain.Users.Student;

namespace ITMO.Dev.ASAP.Application.Services;

public class SubjectCourseService : ISubjectCourseService
{
    private readonly IDatabaseContext _context;
    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly IGithubUserService _githubUserService;

    public SubjectCourseService(
        IDatabaseContext context,
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
        List<Assignment> assignments = await _context.Assignments
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Group)
            .ThenInclude(x => x.Students)
            .ThenInclude(x => x.User)
            .ThenInclude(x => x.Associations)
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Submissions)
            .AsSplitQuery()
            .Where(x => x.SubjectCourse.Id.Equals(subjectCourseId))
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        IEnumerable<StudentAssignment> studentAssignmentPoints = assignments
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(ga => ga.Group.Students.Select(s => new StudentAssignment(s, ga)));

        List<StudentPointsDto> studentPoints = await studentAssignmentPoints
            .GroupBy(x => x.Student)
            .ToAsyncEnumerable()
            .SelectAwait(async x => await MapToStudentPoints(x, cancellationToken))
            .OrderBy(x => x.Student.GroupName)
            .ThenBy(x => _userFullNameFormatter.GetFullName(x.Student.User))
            .ToListAsync(cancellationToken);

        AssignmentDto[] assignmentsDto = assignments.Select(x => x.ToDto()).ToArray();
        return new SubjectCoursePointsDto(assignmentsDto, studentPoints);
    }

    private async Task<StudentPointsDto> MapToStudentPoints(
        IGrouping<Student, StudentAssignment> grouping,
        CancellationToken cancellationToken)
    {
        GithubUserDto? githubUser = await _githubUserService.FindByIdAsync(grouping.Key.UserId, cancellationToken);
        StudentDto studentDto = grouping.Key.ToDto(githubUser?.Username);

        AssignmentPointsDto[] pointsDto = grouping
            .Select(x => x.Points)
            .WhereNotNull()
            .Select(x => new AssignmentPointsDto(x.Assignment.Id, x.SubmissionDate, x.IsBanned, x.Points.Value))
            .ToArray();

        return new StudentPointsDto(studentDto, pointsDto);
    }
}