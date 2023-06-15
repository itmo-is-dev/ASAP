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
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using MediatR;

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
    private readonly IPersistenceContext _context;
    private readonly IQueueUpdateService _queueUpdateService;
    private readonly ISubjectCourseUpdateService _subjectCourseUpdateService;

    public TableUpdateNotificationHandler(
        IPersistenceContext context,
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
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByAssignmentId(
            notification.GroupAssignment.AssignmentId,
            cancellationToken);

        _subjectCourseUpdateService.UpdatePoints(subjectCourse.Id);
        _queueUpdateService.Update(subjectCourse.Id, notification.GroupAssignment.GroupId);
    }

    public async Task Handle(StudyGroupUpdated.Notification notification, CancellationToken cancellationToken)
    {
        var query = SubjectCourseQuery.Build(x => x.WithStudentGroupId(notification.Group.Id));

        SubjectCourse[] courses = await _context.SubjectCourses
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

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
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        _subjectCourseUpdateService.UpdatePoints(subjectCourse.Id);
    }

    public async Task Handle(SubmissionStateUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        StudentGroup group = await _context.StudentGroups.GetByStudentId(
            notification.Submission.StudentId,
            cancellationToken);

        _queueUpdateService.Update(subjectCourse.Id, group.Id);
    }

    public async Task Handle(SubmissionUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        StudentGroup group = await _context.StudentGroups.GetByStudentId(
            notification.Submission.StudentId,
            cancellationToken);

        _subjectCourseUpdateService.UpdatePoints(subjectCourse.Id);
        _queueUpdateService.Update(subjectCourse.Id, group.Id);
    }

    public async Task Handle(StudentTransferred.Notification notification, CancellationToken cancellationToken)
    {
        var queryBuilder = new SubjectCourseQuery.Builder();

        queryBuilder.WithStudentGroupId(notification.NewGroupId);

        if (notification.OldGroupId is not null)
        {
            queryBuilder.WithStudentGroupId(notification.OldGroupId.Value);
        }

        IAsyncEnumerable<SubjectCourse> subjectCourses = _context.SubjectCourses
            .QueryAsync(queryBuilder.Build(), cancellationToken);

        IAsyncEnumerable<(Guid SubjectCourseId, Guid GroupId)> pairsEnumerable = subjectCourses.SelectMany(
            x => x.Groups.ToAsyncEnumerable(),
            (x, group) => (subjectCourseId: x.Id, group.Id));

        if (notification.OldGroupId is null)
        {
            pairsEnumerable = pairsEnumerable.Where(x => x.GroupId.Equals(notification.NewGroupId));
        }
        else
        {
            pairsEnumerable = pairsEnumerable.Where(x =>
                x.GroupId.Equals(notification.NewGroupId)
                || x.GroupId.Equals(notification.OldGroupId.Value));
        }

        (Guid SubjectCourseId, Guid GroupId)[] pairs = await pairsEnumerable.ToArrayAsync(cancellationToken);

        IEnumerable<Guid> subjectCourseIds = pairs
            .Select(x => x.SubjectCourseId)
            .Distinct();

        foreach (Guid subjectCourse in subjectCourseIds)
        {
            _subjectCourseUpdateService.UpdatePoints(subjectCourse);
        }

        foreach ((Guid subjectCourseId, Guid groupId) in pairs)
        {
            _queueUpdateService.Update(subjectCourseId, groupId);
        }
    }
}