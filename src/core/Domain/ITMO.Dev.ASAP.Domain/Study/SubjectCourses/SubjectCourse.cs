using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Domain.Users;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses;

public partial class SubjectCourse : IEntity<Guid>
{
    private readonly HashSet<Mentor> _mentors;
    private readonly HashSet<StudentGroupInfo> _groups;
    private readonly HashSet<SubjectCourseAssignment> _assignments;

    public SubjectCourse(
        Guid id,
        Guid subjectId,
        string title,
        SubmissionStateWorkflowType? workflowType,
        HashSet<StudentGroupInfo> groups,
        HashSet<DeadlinePenalty> deadlinePenalties,
        HashSet<SubjectCourseAssignment> assignments,
        HashSet<Mentor> mentors) : this(id)
    {
        SubjectId = subjectId;
        Title = title;
        WorkflowType = workflowType;
        _groups = groups;
        _assignments = assignments;

        _mentors = mentors;
        DeadlinePolicy = new DeadlinePolicy(deadlinePenalties);
    }

    public Guid SubjectId { get; protected init; }

    public string Title { get; private set; }

    public SubmissionStateWorkflowType? WorkflowType { get; set; }

    public IReadOnlyCollection<Mentor> Mentors => _mentors;

    public IReadOnlyCollection<StudentGroupInfo> Groups => _groups;

    public IReadOnlyCollection<SubjectCourseAssignment> Assignments => _assignments;

    public DeadlinePolicy DeadlinePolicy { get; private set; }

    public override string ToString()
    {
        return Title;
    }

    public TitleUpdatedEvent UpdateTitle(string value)
    {
        Title = value;
        return new TitleUpdatedEvent(this, Title);
    }

    public (SubjectCourseGroup Group, ISubjectCourseEvent Event) AddGroup(StudentGroup group)
    {
        if (_groups.Contains(group.Info))
            throw new DomainInvalidOperationException($"Group {group} is already assigned to this course");

        var subjectCourseGroup = new SubjectCourseGroup(subjectCourseId: Id, studentGroupId: group.Id);
        _groups.Add(group.Info);

        IEnumerable<GroupAssignmentCreatedEvent> assignmentEvents = _assignments
            .Select(x => x.AddGroup(group.Info, DateOnly.FromDateTime(DateTime.UnixEpoch)))
            .Select(x => new GroupAssignmentCreatedEvent(x));

        var evt = AggregateSubjectCourseEvent.Build(x => x
            .WithEvent(new SubjectCourseGroupCreatedEvent(subjectCourseGroup))
            .WithEvents(assignmentEvents));

        return (subjectCourseGroup, evt);
    }

    public (IReadOnlyCollection<SubjectCourseGroup> Groups, ISubjectCourseEvent Event) AddGroups(
        IEnumerable<StudentGroup> groups)
    {
        IEnumerable<Guid> groupIds = _groups.Select(x => x.Id);
        IEnumerable<StudentGroup> groupsToAdd = groups.ExceptBy(groupIds, x => x.Id);

        var subjectCourseGroups = new List<SubjectCourseGroup>();
        var eventBuilder = new AggregateSubjectCourseEvent.Builder();

        foreach (StudentGroup studentGroup in groupsToAdd)
        {
            var subjectCourseGroup = new SubjectCourseGroup(subjectCourseId: Id, studentGroupId: studentGroup.Id);
            subjectCourseGroups.Add(subjectCourseGroup);

            IEnumerable<GroupAssignmentCreatedEvent> assignmentEvents = _assignments
                .Select(x => x.AddGroup(studentGroup.Info, DateOnly.FromDateTime(DateTime.UnixEpoch)))
                .Select(x => new GroupAssignmentCreatedEvent(x));

            var evt = new SubjectCourseGroupCreatedEvent(subjectCourseGroup);

            eventBuilder = eventBuilder.WithEvent(evt).WithEvents(assignmentEvents);
        }

        return (subjectCourseGroups, eventBuilder.Build());
    }

    public ISubjectCourseEvent RemoveGroup(Guid groupId)
    {
        if (_groups.Any(x => x.Id.Equals(groupId)) is false)
            throw new DomainInvalidOperationException($"Group {groupId} is not assigned to this course");

        return new SubjectCourseGroupRemovedEvent(this, groupId);
    }

    public (ISubjectCourseEvent Event, Assignment Assignment) AddAssignment(Assignment assignment)
    {
        var subjectCourseAssignment = new SubjectCourseAssignment(assignment.Info, new HashSet<StudentGroupInfo>());
        _assignments.Add(subjectCourseAssignment);

        var evt = new SubjectCourseAssignmentCreatedEvent(this, assignment);

        IEnumerable<GroupAssignmentCreatedEvent> events = _groups
            .Select(x => subjectCourseAssignment.AddGroup(x, DateOnly.FromDateTime(DateTime.UnixEpoch)))
            .Select(x => new GroupAssignmentCreatedEvent(x));

        var aggregateEvt = AggregateSubjectCourseEvent.Build(x => x.WithEvent(evt).WithEvents(events));

        return (aggregateEvt, assignment);
    }

    public ISubjectCourseEvent UpdateMentors(IReadOnlyCollection<User> users)
    {
        var eventBuilder = new AggregateSubjectCourseEvent.Builder();

        IEnumerable<Guid> userIds = users.Select(x => x.Id);
        IEnumerable<Mentor> mentorsToRemove = _mentors.ExceptBy(userIds, x => x.UserId);

        foreach (Mentor mentor in mentorsToRemove)
        {
            _mentors.Remove(mentor);
            eventBuilder.WithEvent(new MentorRemovedEvent(mentor));
        }

        IEnumerable<Guid> mentorIds = _mentors.Select(x => x.UserId);
        IEnumerable<User> mentorsToAddIds = users.ExceptBy(mentorIds, x => x.Id);

        foreach (User user in mentorsToAddIds)
        {
            var mentor = new Mentor(user.Id, Id);
            _mentors.Add(mentor);

            eventBuilder.WithEvent(new MentorAddedEvent(mentor));
        }

        return eventBuilder.Build();
    }

    public DeadlinePenaltyAddedEvent AddDeadlinePenalty(DeadlinePenalty penalty)
    {
        DeadlinePolicy.AddDeadlinePenalty(penalty);
        return new DeadlinePenaltyAddedEvent(this, penalty);
    }
}