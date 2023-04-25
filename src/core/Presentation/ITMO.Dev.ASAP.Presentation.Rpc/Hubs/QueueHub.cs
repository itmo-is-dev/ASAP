using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Hubs;

public class QueueHub : Hub<IQueueHubClient>
{
    private readonly IMediator _mediator;
    private readonly ILogger<QueueHub> _logger;

    public QueueHub(IMediator mediator, ILogger<QueueHub> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task QueueUpdateSubscribeAsync(Guid courseId, Guid groupId)
    {
        await ExecuteSafeAsync(async () =>
        {
            var query = new HasAccessToSubjectCourse.Query(courseId);
            HasAccessToSubjectCourse.Response response = await _mediator.Send(query, Context.ConnectionAborted);

            if (response.HasAccess)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, CombineIdentifiers(courseId, groupId));
            }
        });
    }

    public Task QueueUpdateUnsubscribeAsync(Guid courseId, Guid groupId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, CombineIdentifiers(courseId, groupId));
    }

    private static string CombineIdentifiers(Guid courseId, Guid groupId)
    {
        return string.Concat(courseId, groupId);
    }

    private async Task ExecuteSafeAsync(Func<Task> func)
    {
        try
        {
            await func.Invoke();
        }
        catch (NotFoundException)
        {
            await Clients.Caller.SendError("Requested subject course does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to execute hub request");
            await Clients.Caller.SendError("Failed to process request");
        }
    }
}