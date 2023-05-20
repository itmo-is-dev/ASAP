using ITMO.Dev.ASAP.Github.Domain.Assignments;

namespace ITMO.Dev.ASAP.Github.DataAccess.Models;

public record GithubAssignmentModel(Guid Id, Guid SubjectCourseId, string BranchName)
{
    public GithubAssignment ToEntity()
    {
        return new GithubAssignment(Id, SubjectCourseId, BranchName);
    }
}