using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Queue;
using ITMO.Dev.ASAP.Domain.Queue.Building;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Application.Queue;

#pragma warning disable CA1506
public class QueueService : IQueueService
{
    private readonly IPersistenceContext _context;
    private readonly IGithubUserService _githubUserService;

    public QueueService(
        IPersistenceContext context,
        IGithubUserService githubUserService)
    {
        _context = context;
        _githubUserService = githubUserService;
    }

    public async Task<SubmissionsQueueDto> GetSubmissionsQueueAsync(
        Guid subjectCourseId,
        Guid studentGroupId,
        CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(studentGroupId, cancellationToken);

        SubmissionQueue queue = new DefaultQueueBuilder(studentGroupId, subjectCourseId).Build();

        var filterVisitor = new FilterCriteriaVisitor(new SubmissionQuery.Builder());
        queue.AcceptFilterCriteriaVisitor(filterVisitor);

        IAsyncEnumerable<Submission> submissionsEnumerable = _context.Submissions
            .QueryAsync(filterVisitor.Builder.Build(), cancellationToken);

        var evaluatorVisitor = new EvaluatorCriteriaVisitor(_context, subjectCourseId);
        submissionsEnumerable = queue.OrderSubmissionsAsync(submissionsEnumerable, evaluatorVisitor, cancellationToken);

        Submission[] submissions = await submissionsEnumerable.ToArrayAsync(cancellationToken);

        IEnumerable<Guid> assignmentIds = submissions.Select(x => x.GroupAssignment.Assignment.Id).Distinct();

        Assignment[] assignments = await _context.Assignments
            .GetByIdsAsync(assignmentIds, cancellationToken)
            .ToArrayAsync(cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(subjectCourseId, cancellationToken);

        RatedSubmission[] ratedSubmissions = submissions
            .Join(
                assignments,
                x => x.GroupAssignment.Assignment.Id,
                x => x.Id,
                (s, a) => (s, a))
            .Select(x => x.s.CalculateEffectivePoints(x.a, subjectCourse.DeadlinePolicy))
            .ToArray();

        IReadOnlyList<QueueSubmissionDto> submissionsDto = await _githubUserService
            .MapToQueueSubmissionDto(ratedSubmissions, cancellationToken);

        return new SubmissionsQueueDto(group.Name, submissionsDto);
    }
}