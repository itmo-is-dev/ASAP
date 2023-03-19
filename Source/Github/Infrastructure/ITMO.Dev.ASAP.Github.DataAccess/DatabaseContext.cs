using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Github.DataAccess;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DbSet<GithubUser> Users => Set<GithubUser>();

    public DbSet<GithubSubmission> Submissions => Set<GithubSubmission>();

    public DbSet<GithubSubjectCourse> SubjectCourses => Set<GithubSubjectCourse>();

    public DbSet<GithubAssignment> Assignments => Set<GithubAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemblyMarker).Assembly);
}