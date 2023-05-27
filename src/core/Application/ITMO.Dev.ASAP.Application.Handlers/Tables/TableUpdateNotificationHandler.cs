using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Contracts.Students.Notifications;
using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Notifications;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Notifications;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Domain.Study;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Handlers.Tables;

internal class TableUpdateNotificationHandler :
    INotificationHandler<AssignmentCreated.Notification>,
    INotificationHandler<AssignmentPointsUpdated.Notification>,
    INotificationHandler<GroupAssignmentDeadlineUpdated.Notification>,
    INotificationHandler<StudyGroupUpdated.Notification>,
    INotificationHandler<SubjectCourseGroupCreated.Notification>,
    INotificationHandler<SubjectCourseGroupDeleted.Notification>,
    INotificationHandler<DeadlinePolicyAdded.Notification>,
    INotificationHandler<SubmissionPointsUpdated.Notification>,
    INotificationHandler<SubmissionStateUpdated.Notification>,
    INotificationHandler<SubmissionUpdated.Notification>,
    INotificationHandler<StudentTransferred.Notification>
{
    private readonly IDatabaseContext _context;
    private readonly IQueueUpdateService _queueUpdateService;
    private readonly ISubjectCourseUpdateService _subjectCourseUpdateService;

    public TableUpdateNotificationHandler(
        IDatabaseContext context,
        IQueueUpdateService queueUpdateService,
        ISubjectCourseUpdateService subjectCourseUpdateService)
    {
        _context = context;
        _queueUpdateService = queueUpdateService;
        _subjectCourseUpdateService = subjectCourseUpdateService;
    }

    public Task Handle(AssignmentCreated.Notification notification, CancellationToken cancellationToken)
    {
        _subjectCourseUpdateService.UpdatePoints(notification.Assignment.SubjectCourseId);
        return Task.CompletedTask;
    }

    public Task Handle(AssignmentPointsUpdated.Notification notification, CancellationToken cancellationToken)
    {
        _subjectCourseUpdateService.UpdatePoints(notification.Assignment.SubjectCourseId);
        return Task.CompletedTask;
    }

    public async Task Handle(
        GroupAssignmentDeadlineUpdated.Notification notification,
        CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.GroupAssignment.AssignmentId,
            cancellationToken);

        _subjectCourseUpdateService.UpdatePoints(subjectCourse.Id);
        _queueUpdateService.Update(subjectCourse.Id, notification.GroupAssignment.GroupId);
    }

    public async Task Handle(StudyGroupUpdated.Notification notification, CancellationToken cancellationToken)
    {
        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(sc => sc.Groups.Any(g => g.StudentGroupId.Equals(notification.Group.Id)))
            .ToListAsync(cancellationToken);

        foreach (SubjectCourse course in courses)
        {
            _subjectCourseUpdateService.UpdatePoints(course.Id);
        }
    }

    public Task Handle(SubjectCourseGroupCreated.Notification notification, CancellationToken cancellationToken)
    {
        (Guid subjectCourseId, Guid groupId) = notification.Group;
        _queueUpdateService.Update(subjectCourseId, groupId);

        return Task.CompletedTask;
    }

    public Task Handle(SubjectCourseGroupDeleted.Notification notification, CancellationToken cancellationToken)
    {
        _queueUpdateService.Update(notification.SubjectCourseId, notification.GroupId);
        return Task.CompletedTask;
    }

    public Task Handle(DeadlinePolicyAdded.Notification notification, CancellationToken cancellationToken)
    {
        _subjectCourseUpdateService.UpdatePoints(notification.SubjectCourseId);
        return Task.CompletedTask;
    }

    public async Task Handle(SubmissionPointsUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        _subjectCourseUpdateService.UpdatePoints(subjectCourse.Id);
    }

    public async Task Handle(SubmissionStateUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        StudentGroup group = await _context.GetStudentGroupByStudentId(
            notification.Submission.StudentId,
            cancellationToken);

        _queueUpdateService.Update(subjectCourse.Id, group.Id);
    }

    public async Task Handle(SubmissionUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        StudentGroup group = await _context.GetStudentGroupByStudentId(
            notification.Submission.StudentId,
            cancellationToken);

        _subjectCourseUpdateService.UpdatePoints(subjectCourse.Id);
        _queueUpdateService.Update(subjectCourse.Id, group.Id);
    }

    public async Task Handle(StudentTransferred.Notification notification, CancellationToken cancellationToken)
    {
        var subjectCoursesQuery = _context.SubjectCourses
            .Where(sc => sc.Groups.Any(g => g.StudentGroupId.Equals(notification.NewGroupId)))
            .Select(x => new { x.Id, GroupId = notification.NewGroupId });

        if (notification.OldGroupId is not null)
        {
            var oldGroupSubjectCourses = _context.SubjectCourses
                .Where(sc => sc.Groups.Any(g => g.StudentGroupId.Equals(notification.OldGroupId)))
                .Select(x => new { x.Id, GroupId = notification.OldGroupId.Value });

            subjectCoursesQuery = subjectCoursesQuery.Union(oldGroupSubjectCourses);
        }

        var pairs = await subjectCoursesQuery.ToListAsync(cancellationToken);
        IEnumerable<Guid> subjectCourses = pairs.Select(x => x.Id).Distinct();

        foreach (Guid subjectCourse in subjectCourses)
        {
            _subjectCourseUpdateService.UpdatePoints(subjectCourse);
        }

        foreach (var pair in pairs)
        {
            _queueUpdateService.Update(pair.Id, pair.GroupId);
        }
    }
}