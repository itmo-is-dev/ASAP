using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

#pragma warning disable CA1506
public class StudentAssignmentRepository : IStudentAssignmentRepository
{
    private readonly DatabaseContext _context;
    private readonly ISubjectCourseRepository _subjectCourseRepository;

    public StudentAssignmentRepository(DatabaseContext context, ISubjectCourseRepository subjectCourseRepository)
    {
        _context = context;
        _subjectCourseRepository = subjectCourseRepository;
    }

    public async IAsyncEnumerable<StudentAssignment> GetBySubjectCourseIdAsync(
        Guid subjectCourseId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _subjectCourseRepository
            .QueryAsync(SubjectCourseQuery.Build(x => x.WithId(subjectCourseId)), cancellationToken)
            .SingleAsync(cancellationToken);

        List<AssignmentModel> assignmentModels = await _context.Assignments
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.StudentGroup)
            .ThenInclude(x => x.Students)
            .ThenInclude(x => x.User)
            .ThenInclude(x => x.Associations)
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Submissions)
            .ThenInclude(x => x.Student)
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.StudentGroup)
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Assignment)
            .AsSplitQuery()
            .AsNoTrackingWithIdentityResolution()
            .Where(x => x.SubjectCourse.Id.Equals(subjectCourseId))
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        IEnumerable<Student> students = assignmentModels
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(x => x.StudentGroup.Students)
            .Distinct()
            .Select(StudentMapper.MapTo);

        IEnumerable<Assignment> assignments = assignmentModels.Select(AssignmentMapper.MapTo);

        GroupAssignment[] groupAssignments = assignmentModels
            .SelectMany(x => x.GroupAssignments)
            .Select(x => GroupAssignmentMapper
                .MapTo(x, x.StudentGroup.Name, x.Assignment.Title, x.Assignment.ShortName))
            .ToArray();

        Submission[] submissions = assignmentModels
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(x => x.Submissions, (ga, s) => (ga, s))
            .Select(x =>
            {
                GroupAssignment groupAssignment = GroupAssignmentMapper.MapTo(
                    x.ga,
                    x.ga.StudentGroup.Name,
                    x.ga.Assignment.Title,
                    x.ga.Assignment.ShortName);

                return SubmissionMapper.MapTo(x.s, groupAssignment);
            })
            .ToArray();

        IEnumerable<(Student Student, Assignment Assignment)> enumerable = students
            .SelectMany(_ => assignments, (student, assignment) => (student, assignment));

        foreach ((Student student, Assignment assignment) in enumerable)
        {
            GroupAssignment[] ga = groupAssignments
                .Where(x => x.Id.AssignmentId.Equals(assignment.Id))
                .ToArray();

            Submission[] s = submissions
                .Where(x => x.Student.Equals(student))
                .Where(x => x.GroupAssignment.Id.AssignmentId.Equals(assignment.Id))
                .ToArray();

            yield return new StudentAssignment(student, assignment, ga, s, subjectCourse);
        }
    }
}