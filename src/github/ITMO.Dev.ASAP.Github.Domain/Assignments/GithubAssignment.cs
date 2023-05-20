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

    public Guid SubjectCourseId { get; set; }

    public string BranchName { get; set; }
}