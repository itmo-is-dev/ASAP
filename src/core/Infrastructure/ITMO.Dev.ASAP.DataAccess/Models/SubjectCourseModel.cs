using ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models;

public partial class SubjectCourseModel : IEntity<Guid>
{
    public SubjectCourseModel(
        Guid id,
        Guid subjectId,
        string title,
        SubmissionStateWorkflowType? workflowType) : this(id)
    {
        SubjectId = subjectId;
        Title = title;
        WorkflowType = workflowType;
    }

    public Guid SubjectId { get; set; }

    public virtual SubjectModel Subject { get; set; }

    public string Title { get; set; }

    public SubmissionStateWorkflowType? WorkflowType { get; set; }

    public virtual ICollection<AssignmentModel> Assignments { get; init; }

    public virtual ICollection<DeadlinePenaltyModel> DeadlinePenalties { get; init; }

    public virtual ICollection<MentorModel> Mentors { get; init; }

    public virtual ICollection<SubjectCourseGroupModel> SubjectCourseGroups { get; init; }
}