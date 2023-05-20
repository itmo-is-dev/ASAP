using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using System.Data;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess;

public interface IPersistenceContext
{
    IGithubAssignmentRepository Assignments { get; }

    IGithubSubmissionRepository Submissions { get; }

    IGithubUserRepository Users { get; }

    IGithubSubjectCourseRepository SubjectCourses { get; }

    Task CommitAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);
}