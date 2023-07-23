using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Assignments;

public class GroupAssignmentFactory
{
    private readonly IMessageProvider _provider;
    private readonly IMessagePublisher _publisher;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGroupAssignmentClient _groupAssignmentClient;

    public GroupAssignmentFactory(
        IMessageProvider provider,
        IMessagePublisher publisher,
        ISafeExecutor safeExecutor,
        IGroupAssignmentClient groupAssignmentClient)
    {
        _provider = provider;
        _publisher = publisher;
        _safeExecutor = safeExecutor;
        _groupAssignmentClient = groupAssignmentClient;
    }

    public IGroupAssignment Create(GroupAssignmentDto groupAssignment)
    {
        return new GroupAssignment(groupAssignment, _provider, _publisher, _safeExecutor, _groupAssignmentClient);
    }
}