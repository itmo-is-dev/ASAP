using MediatR;

namespace ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;

public record SubjectCourseGroupQueueUpdateNotification(Guid SubjectCourseId, Guid GroupId) : INotification;