using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class SubmissionSpecification
{
    public static IQueryable<Submission> ForUser(this IQueryable<Submission> queryable, Guid userId)
        => queryable.Where(x => x.Student.UserId.Equals(userId));

    public static IQueryable<Submission> ForAssignment(this IQueryable<Submission> queryable, Guid submissionId)
        => queryable.Where(x => x.GroupAssignment.AssignmentId.Equals(submissionId));

    public static IIncludableQueryable<Submission, SubjectCourse> IncludeSubjectCourse(
        this IQueryable<Submission> query)
    {
        return query.Include(x => x.GroupAssignment.Assignment.SubjectCourse);
    }

    public static IIncludableQueryable<Submission, StudentGroup?> IncludeStudentGroup(this IQueryable<Submission> query)
    {
        return query.Include(x => x.Student.Group);
    }

    public static async Task<Submission> GetSubmissionForCodeOrLatestAsync(
        this IQueryable<Submission> queryable,
        Guid userId,
        Guid assignmentId,
        int? code,
        CancellationToken cancellationToken)
    {
        IQueryable<Submission> submissionsQuery = queryable
            .IncludeSubjectCourse()
            .IncludeStudentGroup()
            .Where(x => x.Student.UserId.Equals(userId))
            .Where(x => x.GroupAssignment.AssignmentId.Equals(assignmentId));

        Submission? submission = code is null
            ? await submissionsQuery.OrderByDescending(x => x.Code).FirstOrDefaultAsync(cancellationToken)
            : await submissionsQuery.Where(x => x.Code.Equals(code)).SingleOrDefaultAsync(cancellationToken);

        return submission ?? throw new EntityNotFoundException("Could not find submission");
    }
}