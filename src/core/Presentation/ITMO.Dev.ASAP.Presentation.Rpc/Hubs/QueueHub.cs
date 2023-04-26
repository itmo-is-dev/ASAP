using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Hubs;

[Authorize(Roles = AsapIdentityRoleNames.AtLeastMentor)]
public class QueueHub : Hub<IQueueHubClient>
{
    private readonly ILogger<QueueHub> _logger;

    public QueueHub(ILogger<QueueHub> logger)
    {
        _logger = logger;
    }

    public async Task QueueUpdateSubscribeAsync(Guid courseId, Guid groupId)
    {
        IServiceProvider? services = Context.GetHttpContext()?.RequestServices;

        if (services is null)
            return;

        await ExecuteSafeAsync(async () =>
        {
            IMediator mediator = services.GetRequiredService<IMediator>();

            var query = new HasAccessToSubjectCourse.Query(courseId);
            HasAccessToSubjectCourse.Response response = await mediator.Send(query, Context.ConnectionAborted);

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