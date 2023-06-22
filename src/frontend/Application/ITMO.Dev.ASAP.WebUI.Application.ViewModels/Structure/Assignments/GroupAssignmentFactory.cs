using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Assignments;

public class GroupAssignmentFactory
{
    private readonly IMessageProducer _producer;
    private readonly IMessageConsumer _consumer;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGroupAssignmentClient _groupAssignmentClient;

    public GroupAssignmentFactory(
        IMessageProducer producer,
        IMessageConsumer consumer,
        ISafeExecutor safeExecutor,
        IGroupAssignmentClient groupAssignmentClient)
    {
        _producer = producer;
        _consumer = consumer;
        _safeExecutor = safeExecutor;
        _groupAssignmentClient = groupAssignmentClient;
    }

    public IGroupAssignment Create(GroupAssignmentDto groupAssignment)
    {
        return new GroupAssignment(groupAssignment, _producer, _consumer, _safeExecutor, _groupAssignmentClient);
    }
}