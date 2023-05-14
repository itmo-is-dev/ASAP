using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess;

public interface IPersistenceContext
{
    IGithubAssignmentRepository Assignments { get; }

    IGithubSubmissionRepository Submissions { get; }

    IGithubUserRepository Users { get; }

    IGithubSubjectCourseRepository SubjectCourses { get; }

    Task CommitAsync(CancellationToken cancellationToken);
}