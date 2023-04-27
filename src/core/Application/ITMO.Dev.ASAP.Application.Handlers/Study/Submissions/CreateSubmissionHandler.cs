using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.CreateSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class CreateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public CreateSubmissionHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        (Guid issuerId, Guid studentId, Guid assignmentId, string payload) = request;

        Student? student = await _context.Assignments
            .Where(x => x.Id.Equals(assignmentId))
            .SelectMany(x => x.SubjectCourse.Groups)
            .SelectMany(x => x.StudentGroup.Students)
            .Where(x => x.UserId.Equals(issuerId))
            .SingleOrDefaultAsync(cancellationToken);

        // If issuer is not a student, check if it is mentor and find student corresponding to the repository
        if (student is null)
        {
            IQueryable<SubjectCourse> subjectCourseQuery = _context.Assignments
                .Where(x => x.Id.Equals(assignmentId))
                .Select(x => x.SubjectCourse);

            Mentor? mentor = await subjectCourseQuery
                .SelectMany(x => x.Mentors)
                .Where(x => x.UserId.Equals(issuerId))
                .SingleOrDefaultAsync(cancellationToken);

            if (mentor is not null)
            {
                student = await subjectCourseQuery
                    .SelectMany(x => x.Groups)
                    .SelectMany(x => x.StudentGroup.Students)
                    .Where(x => x.UserId.Equals(studentId))
                    .SingleOrDefaultAsync(cancellationToken);

                if (student is null)
                    throw EntityNotFoundException.For<Student>(studentId);
            }
            else
            {
                SubjectCourse? subjectCourse = await subjectCourseQuery.SingleOrDefaultAsync(cancellationToken);

                if (subjectCourse is null)
                    throw EntityNotFoundException.For<Assignment>(assignmentId);

                throw EntityNotFoundException.UserNotFoundInSubjectCourse(studentId, subjectCourse.Title);
            }
        }

        GroupAssignment groupAssignment = await _context.GroupAssignments
            .WithAssignment(assignmentId)
            .ForStudent(student.UserId)
            .SingleAsync(cancellationToken);

        int count = await _context.Submissions
            .ForAssignment(assignmentId)
            .ForUser(student.UserId)
            .CountAsync(cancellationToken);

        var submission = new Submission(
            Guid.NewGuid(),
            count + 1,
            student,
            groupAssignment,
            Calendar.CurrentDateTime,
            payload);

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionDto dto = submission.ToDto();

        return new Response(dto);
    }
}