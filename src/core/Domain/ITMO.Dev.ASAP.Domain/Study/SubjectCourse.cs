using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Domain.Users;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study;

public partial class SubjectCourse : IEntity<Guid>
{
    private readonly HashSet<Assignment> _assignments;
    private readonly HashSet<SubjectCourseAssociation> _associations;
    private readonly HashSet<DeadlinePolicy> _deadlinePolicies;
    private readonly HashSet<SubjectCourseGroup> _groups;
    private readonly HashSet<Mentor> _mentors;

    public SubjectCourse(Guid id, Subject subject, string title, SubmissionStateWorkflowType? workflowType)
        : this(id)
    {
        Subject = subject;
        Title = title;
        WorkflowType = workflowType;

        _groups = new HashSet<SubjectCourseGroup>();
        _assignments = new HashSet<Assignment>();
        _associations = new HashSet<SubjectCourseAssociation>();
        _mentors = new HashSet<Mentor>();

        // TODO: change when deadline policy customization is implemented
        _deadlinePolicies = Enumerable
            .Range(0, 5)
            .Select<int, DeadlinePolicy>(i => new FractionDeadlinePolicy(TimeSpan.FromDays(7) * i, 1 - (0.2 * (i + 1))))
            .ToHashSet();
    }

    public virtual Subject Subject { get; protected init; }

    public string Title { get; set; }

    public SubmissionStateWorkflowType? WorkflowType { get; set; }

    public virtual IReadOnlyCollection<SubjectCourseGroup> Groups => _groups;

    public virtual IReadOnlyCollection<Assignment> Assignments => _assignments;

    public virtual IReadOnlyCollection<SubjectCourseAssociation> Associations => _associations;

    public virtual IReadOnlyCollection<Mentor> Mentors => _mentors;

    public virtual IReadOnlyCollection<DeadlinePolicy> DeadlinePolicies => _deadlinePolicies;

    public override string ToString()
    {
        return Title;
    }

    public SubjectCourseGroup AddGroup(StudentGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (Groups.Any(x => x.StudentGroup.Equals(group)))
            throw new DomainInvalidOperationException($"Group {group} is already assigned to this course");

        var subjectCourseGroup = new SubjectCourseGroup(this, group);
        _groups.Add(subjectCourseGroup);

        foreach (Assignment assignment in Assignments)
        {
            assignment.AddGroup(group, DateOnly.FromDateTime(DateTime.UnixEpoch));
        }

        return subjectCourseGroup;
    }

    public void RemoveGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_groups.Remove(group))
            throw new DomainInvalidOperationException($"Group {group} is not assigned to this course");
    }

    public void AddAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Add(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} is already assigned to this course");

        foreach (SubjectCourseGroup group in Groups)
        {
            assignment.AddGroup(group.StudentGroup, DateOnly.FromDateTime(DateTime.UnixEpoch));
        }
    }

    public void RemoveAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Remove(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} is not assigned to this course");
    }

    public void AddAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        Type associationType = association.GetType();

        if (Assignments.Any(a => a.GetType() == associationType))
            throw new DomainInvalidOperationException($"Course {this} already has {associationType} association");

        _associations.Add(association);
    }

    public void RemoveAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            throw new DomainInvalidOperationException($"Association {association} is not assigned to this course");
    }

    public Mentor AddMentor(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (Mentors.Any(x => x.User.Equals(user)))
            throw new DomainInvalidOperationException($"User {user} is already a mentor of this subject course");

        var mentor = new Mentor(user, this);
        _mentors.Add(mentor);

        return mentor;
    }

    public void RemoveMentor(Mentor mentor)
    {
        ArgumentNullException.ThrowIfNull(mentor);

        if (!_mentors.Remove(mentor))
            throw new DomainInvalidOperationException($"Mentor {mentor} is not a mentor of this subject course");
    }

    public void AddDeadlinePolicy(DeadlinePolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        if (!_deadlinePolicies.Add(policy))
            throw new DomainInvalidOperationException($"Deadline span {policy} already exists");
    }

    public void RemoveDeadlinePolicy(DeadlinePolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        if (!_deadlinePolicies.Remove(policy))
            throw new DomainInvalidOperationException($"Deadline span {policy} cannot be removed");
    }
}