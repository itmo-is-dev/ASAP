using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Queue;
using ITMO.Dev.ASAP.Domain.Queue.EvaluationCriteria;
using ITMO.Dev.ASAP.Domain.Queue.Models;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Tools;
using System.Runtime.CompilerServices;

namespace ITMO.Dev.ASAP.Application.Queue;

public class EvaluatorCriteriaVisitor : IEvaluationCriteriaVisitor
{
    private readonly IPersistenceContext _context;
    private readonly Guid _subjectCourseId;

    public EvaluatorCriteriaVisitor(IPersistenceContext context, Guid subjectCourseId)
    {
        _context = context;
        _subjectCourseId = subjectCourseId;
    }

    public async IAsyncEnumerable<EvaluatedSubmission> VisitAsync(
        IAsyncEnumerable<Submission> submissions,
        AssignmentDeadlineEvaluationCriteria criteria,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Submission[] submissionsArray = await submissions.ToArrayAsync(cancellationToken);

        GroupAssignment[] groupAssignments = await _context.GroupAssignments
            .QueryAsync(GroupAssignmentQuery.Build(x => x.WithSubjectCourseId(_subjectCourseId)), cancellationToken)
            .ToArrayAsync(cancellationToken);

        IEnumerable<EvaluatedSubmission> evaluatedSubmissions = submissionsArray
            .Join(
                groupAssignments,
                x => x.GroupAssignment.Id,
                x => x.Id,
                (submission, groupAssignment) => (submission, groupAssignment))
            .Select(x => EvaluateSubmission(x.submission, x.groupAssignment, groupAssignments));

        foreach (EvaluatedSubmission evaluatedSubmission in evaluatedSubmissions)
        {
            yield return evaluatedSubmission;
        }
    }

    public IAsyncEnumerable<EvaluatedSubmission> VisitAsync(
        IAsyncEnumerable<Submission> submissions,
        SubmissionDateTimeEvaluationCriteria criteria,
        CancellationToken cancellationToken)
    {
        return submissions.Select(x => new EvaluatedSubmission(x, x.SubmissionDate.Value.Ticks));
    }

    public IAsyncEnumerable<EvaluatedSubmission> VisitAsync(
        IAsyncEnumerable<Submission> submissions,
        SubmissionStateEvaluationCriteria criteria,
        CancellationToken cancellationToken)
    {
        return submissions
            .Select(submission => new EvaluatedSubmission(submission, criteria.RateValue(submission.State.Kind)));
    }

    private EvaluatedSubmission EvaluateSubmission(
        Submission submission,
        GroupAssignment groupAssignment,
        IReadOnlyCollection<GroupAssignment> groupAssignments)
    {
        if (groupAssignment.Deadline < submission.SubmissionDateOnly)
        {
            return new EvaluatedSubmission(
                submission,
                AssignmentDeadlineEvaluationCriteria.ExpiredAssignmentPriority);
        }

        DateOnly now = Calendar.CurrentDate;

        DateOnly closestDeadline = groupAssignments
            .Where(x => x.Id.StudentGroupId.Equals(submission.Student.Group?.Id))
            .Select(x => x.Deadline)
            .Where(x => x >= now)
            .OrderBy(x => x)
            .FirstOrDefault();

        if (groupAssignment.Deadline.Equals(closestDeadline))
        {
            return new EvaluatedSubmission(
                submission,
                AssignmentDeadlineEvaluationCriteria.CurrentAssignmentPriority);
        }

        if (groupAssignment.Deadline >= submission.SubmissionDateOnly)
        {
            return new EvaluatedSubmission(
                submission,
                AssignmentDeadlineEvaluationCriteria.ProperlySubmittedAssignmentPriority);
        }

        return new EvaluatedSubmission(
            submission,
            AssignmentDeadlineEvaluationCriteria.OtherAssignmentPriority);
    }
}