using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;

public static class AssignmentCreated
{
    public record Notification(AssignmentDto Assignment) : INotification;
}