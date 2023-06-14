using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models;

public partial class GroupAssignmentModel : IEntity
{
    public GroupAssignmentModel(Guid studentGroupId, Guid assignmentId, DateOnly deadline)
        : this(studentGroupId: studentGroupId, assignmentId: assignmentId)
    {
        Deadline = deadline;
    }

    [KeyProperty]
    public virtual StudentGroupModel StudentGroup { get; set; }

    [KeyProperty]
    public virtual AssignmentModel Assignment { get; set; }

    public DateOnly Deadline { get; set; }

    public virtual ICollection<SubmissionModel> Submissions { get; init; }
}