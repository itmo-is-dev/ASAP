using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Domain.Assignments;

public partial class GithubAssignment : IEntity<Guid>
{
    public GithubAssignment(Guid id, string branchName) : this(id)
    {
        BranchName = branchName;
    }

    public Guid SubjectCourseId { get; protected init; }

    public string BranchName { get; protected init; }
}