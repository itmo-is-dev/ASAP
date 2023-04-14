using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Core.Queue;
using ITMO.Dev.ASAP.Core.Queue.Building;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Services;

public class QueueUpdateService : IQueueUpdateService
{
    private readonly IDatabaseContext _context;
    private readonly IQueryExecutor _queryExecutor;

    public QueueUpdateService(IDatabaseContext context, IQueryExecutor queryExecutor)
    {
        _context = context;
        _queryExecutor = queryExecutor;
    }

    public async Task<SubmissionsQueueDto> GetSubmmissionsQueue(Guid subjectCourseId, Guid studentGroupId, CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(studentGroupId, cancellationToken);
        SubmissionQueue queue = new DefaultQueueBuilder(group, subjectCourseId).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            _context.Submissions, _queryExecutor, cancellationToken);

        QueueSubmissionDto[] submissionsDto = submissions
            .Select(x => x.ToQueueDto())
            .ToArray();

        string groupName = await _context.StudentGroups
            .Where(x => x.Id.Equals(studentGroupId))
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        return new SubmissionsQueueDto(groupName, submissionsDto);
    }
}