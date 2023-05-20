using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Domain.Assignments;

public partial class GithubAssignment : IEntity<Guid>
{
    public GithubAssignment(Guid id, Guid subjectCourseId, string branchName) : this(id)
    {
        SubjectCourseId = subjectCourseId;
        BranchName = branchName;
        SubjectCourseId = subjectCourseId;
    }

    public Guid SubjectCourseId { get; protected init; }

    public string BranchName { get; protected init; }
}