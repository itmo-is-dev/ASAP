using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;

public static class SubjectCourseCreated
{
    public record Notification(SubjectCourseDto SubjectCourse) : INotification;
}