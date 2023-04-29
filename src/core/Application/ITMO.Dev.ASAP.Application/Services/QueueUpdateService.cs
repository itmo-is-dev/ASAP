using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Domain.Queue;
using ITMO.Dev.ASAP.Domain.Queue.Building;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

namespace ITMO.Dev.ASAP.Application.Services;

public class QueueUpdateService : IQueueUpdateService
{
    private readonly IDatabaseContext _context;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IGithubUserService _githubUserService;

    public QueueUpdateService(
        IDatabaseContext context,
        IQueryExecutor queryExecutor,
        IGithubUserService githubUserService)
    {
        _context = context;
        _queryExecutor = queryExecutor;
        _githubUserService = githubUserService;
    }

    public async Task<SubmissionsQueueDto> GetSubmissionsQueueAsync(
        Guid subjectCourseId,
        Guid studentGroupId,
        CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(studentGroupId, cancellationToken);
        SubmissionQueue queue = new DefaultQueueBuilder(group, subjectCourseId).Build();

        IEnumerable<Submission> submissionsEnumerable = await queue.UpdateSubmissions(
            _context.Submissions,
            _queryExecutor,
            cancellationToken);

        Submission[] submissions = submissionsEnumerable.ToArray();

        IReadOnlyList<QueueSubmissionDto> submissionsDto = await _githubUserService
            .MapToQueueSubmissionDto(submissions, cancellationToken);

        string groupName = await _context.StudentGroups
            .Where(x => x.Id.Equals(studentGroupId))
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        return new SubmissionsQueueDto(groupName, submissionsDto);
    }
}