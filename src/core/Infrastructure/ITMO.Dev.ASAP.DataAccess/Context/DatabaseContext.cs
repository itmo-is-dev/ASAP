using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.ValueConverters;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Submissions.States;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace ITMO.Dev.ASAP.DataAccess.Context;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public DbSet<User> Users { get; protected init; } = null!;

    public DbSet<Student> Students { get; protected init; } = null!;

    public DbSet<Mentor> Mentors { get; protected init; } = null!;

    public DbSet<Assignment> Assignments { get; protected init; } = null!;

    public DbSet<GroupAssignment> GroupAssignments { get; protected init; } = null!;

    public DbSet<StudentGroup> StudentGroups { get; protected init; } = null!;

    public DbSet<Subject> Subjects { get; protected init; } = null!;

    public DbSet<SubjectCourse> SubjectCourses { get; protected init; } = null!;

    public DbSet<SubjectCourseGroup> SubjectCourseGroups { get; protected init; } = null!;

    public DbSet<Submission> Submissions { get; protected init; } = null!;

    public DbSet<UserAssociation> UserAssociations { get; protected init; } = null!;

    public DbSet<DeadlinePenalty> DeadlinePenalties { get; protected init; } = null!;

    public DbSet<DeadlinePolicy> DeadlinePolicies { get; protected init; } = null!;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: WI-228
        IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (IMutableForeignKey fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemblyMarker).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Points>().HaveConversion<PointsValueConverter>();
        configurationBuilder.Properties<Fraction>().HaveConversion<FractionValueConverter>();
        configurationBuilder.Properties<TimeSpan>().HaveConversion<TimeSpanConverter>();

        // configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>();
        configurationBuilder.Properties<SpbDateTime>().HaveConversion<SpbDateTimeValueConverter>();
        configurationBuilder.Properties<ISubmissionState>().HaveConversion<SubmissionStateValueConverter>();
    }
}