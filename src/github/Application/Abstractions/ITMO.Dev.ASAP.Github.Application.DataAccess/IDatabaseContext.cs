using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess;

public interface IDatabaseContext
{
    DbSet<GithubUser> Users { get; }

    DbSet<GithubSubmission> Submissions { get; }

    DbSet<GithubSubjectCourse> SubjectCourses { get; }

    DbSet<GithubAssignment> Assignments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}