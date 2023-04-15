using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Queries.GetSubmissionsQueue;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Queues;

internal class GetSubmissionsQueueHandler : IRequestHandler<Query, Response>
{
    private readonly IQueueUpdateService _queueUpdateService;
    private readonly IMemoryCache _cache;

    public GetSubmissionsQueueHandler(IQueueUpdateService queueUpdateService, IMemoryCache cache)
    {
        _queueUpdateService = queueUpdateService;
        _cache = cache;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        string cacheKey = string.Concat(request.SubjectCourseId, request.GroupId);

        if (_cache.TryGetValue(cacheKey, out SubmissionsQueueDto submissionsQueue))
        {
            return new Response(submissionsQueue);
        }

        submissionsQueue = await _queueUpdateService.GetSubmissionsQueue(
            request.GroupId,
            request.SubjectCourseId,
            cancellationToken);

        _cache.Set(cacheKey, submissionsQueue);

        return new Response(submissionsQueue);
    }
}