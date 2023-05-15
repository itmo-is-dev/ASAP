using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using System.Data;

namespace ITMO.Dev.ASAP.Github.DataAccess;

internal class GithubPersistenceContext : IPersistenceContext
{
    private readonly IUnitOfWork _unitOfWork;

    public GithubPersistenceContext(
        IUnitOfWork unitOfWork,
        IGithubAssignmentRepository assignments,
        IGithubSubmissionRepository submissions,
        IGithubUserRepository users,
        IGithubSubjectCourseRepository subjectCourses)
    {
        _unitOfWork = unitOfWork;
        Assignments = assignments;
        Submissions = submissions;
        Users = users;
        SubjectCourses = subjectCourses;
    }

    public IGithubAssignmentRepository Assignments { get; }

    public IGithubSubmissionRepository Submissions { get; }

    public IGithubUserRepository Users { get; }

    public IGithubSubjectCourseRepository SubjectCourses { get; }

    public Task CommitAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
    {
        return _unitOfWork.CommitAsync(isolationLevel, cancellationToken);
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return _unitOfWork.CommitAsync(cancellationToken);
    }
}