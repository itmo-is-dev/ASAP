using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.DataAccess.ValueConverters;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ITMO.Dev.ASAP.DataAccess.Contexts;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected DatabaseContext(DbContextOptions options) : base(options) { }

    public DbSet<UserModel> Users { get; protected init; } = null!;

    public DbSet<StudentModel> Students { get; protected init; } = null!;

    public DbSet<MentorModel> Mentors { get; protected init; } = null!;

    public DbSet<AssignmentModel> Assignments { get; protected init; } = null!;

    public DbSet<GroupAssignmentModel> GroupAssignments { get; protected init; } = null!;

    public DbSet<StudentGroupModel> StudentGroups { get; protected init; } = null!;

    public DbSet<SubjectModel> Subjects { get; protected init; } = null!;

    public DbSet<SubjectCourseModel> SubjectCourses { get; protected init; } = null!;

    public DbSet<SubjectCourseGroupModel> SubjectCourseGroups { get; protected init; } = null!;

    public DbSet<SubmissionModel> Submissions { get; protected init; } = null!;

    public DbSet<UserAssociationModel> UserAssociations { get; protected init; } = null!;

    public DbSet<DeadlinePenaltyModel> DeadlinePenalties { get; protected init; } = null!;

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

        configurationBuilder.Properties<SpbDateTime>().HaveConversion<SpbDateTimeValueConverter>();
    }
}