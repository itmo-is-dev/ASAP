using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;

public static class SubjectCoursePointsUpdated
{
    public record Notification(Guid SubjectCourseId, SubjectCoursePointsDto Points) : INotification;
}