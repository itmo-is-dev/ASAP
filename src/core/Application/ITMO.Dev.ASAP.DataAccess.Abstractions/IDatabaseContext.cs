using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using ITMO.Dev.ASAP.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ITMO.Dev.ASAP.DataAccess.Abstractions;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }

    DbSet<Student> Students { get; }

    DbSet<Mentor> Mentors { get; }

    DbSet<Assignment> Assignments { get; }

    DbSet<GroupAssignment> GroupAssignments { get; }

    DbSet<StudentGroup> StudentGroups { get; }

    DbSet<Subject> Subjects { get; }

    DbSet<SubjectCourse> SubjectCourses { get; }

    DbSet<SubjectCourseGroup> SubjectCourseGroups { get; }

    DbSet<Submission> Submissions { get; }

    DbSet<UserAssociation> UserAssociations { get; }

    DbSet<DeadlinePenalty> DeadlinePenalties { get; }

    DbSet<DeadlinePolicy> DeadlinePolicies { get;  }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}