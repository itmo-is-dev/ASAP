using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.CreateSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

#pragma warning disable CA1506
internal class CreateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;

    public CreateSubmissionHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        (Guid issuerId, Guid studentId, Guid assignmentId, string payload) = request;

        var studentQuery = StudentQuery.Build(x => x.WithId(request.StudentId).WithAssignmentId(assignmentId));

        Student? student = await _context.Students
            .QueryAsync(studentQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(assignmentId, cancellationToken);

        // If issuer is not a student, check if it is mentor and find student corresponding to the repository
        if (student is null)
        {
            Mentor? mentor = subjectCourse.Mentors.SingleOrDefault(x => x.UserId.Equals(issuerId));

            if (mentor is not null)
            {
                studentQuery = StudentQuery.Build(x => x.WithSubjectCourseId(subjectCourse.Id));

                student = await _context.Students
                    .QueryAsync(studentQuery, cancellationToken)
                    .SingleOrDefaultAsync(cancellationToken);

                if (student is null)
                    throw EntityNotFoundException.For<Student>(studentId);
            }
            else
            {
                throw EntityNotFoundException.UserNotFoundInSubjectCourse(studentId, subjectCourse.Title);
            }
        }

        if (student.Group is null)
        {
            throw new EntityNotFoundException($"Could not find group for student {studentId}");
        }

        GroupAssignment groupAssignment = await _context.GroupAssignments
            .GetByIdsAsync(student.Group.Id, assignmentId, cancellationToken);

        var submissionCountQuery = SubmissionQuery.Build(x => x
            .WithUserId(student.UserId)
            .WithAssignmentId(assignmentId));

        int count = await _context.Submissions.CountAsync(submissionCountQuery, cancellationToken);

        var submission = new Submission(
            Guid.NewGuid(),
            count + 1,
            student,
            Calendar.CurrentDateTime,
            payload,
            groupAssignment);

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        Assignment assignment = await _context.Assignments
            .GetByIdAsync(submission.GroupAssignment.Assignment.Id, cancellationToken);

        Points points = submission.CalculateEffectivePoints(assignment, subjectCourse.DeadlinePolicy).Points;

        SubmissionDto dto = submission.ToDto(points);

        return new Response(dto);
    }
}