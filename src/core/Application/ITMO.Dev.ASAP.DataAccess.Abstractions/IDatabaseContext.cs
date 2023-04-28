using ITMO.Dev.ASAP.Domain.DeadlinePolicies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Assignment = ITMO.Dev.ASAP.Domain.Study.Assignment;
using GroupAssignment = ITMO.Dev.ASAP.Domain.Study.GroupAssignment;
using Mentor = ITMO.Dev.ASAP.Domain.Users.Mentor;
using Student = ITMO.Dev.ASAP.Domain.Users.Student;
using StudentGroup = ITMO.Dev.ASAP.Domain.Study.StudentGroup;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourse;
using SubjectCourseGroup = ITMO.Dev.ASAP.Domain.Study.SubjectCourseGroup;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;
using User = ITMO.Dev.ASAP.Domain.Users.User;
using UserAssociation = ITMO.Dev.ASAP.Domain.UserAssociations.UserAssociation;

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

    DbSet<DeadlinePolicy> DeadlinePolicies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}