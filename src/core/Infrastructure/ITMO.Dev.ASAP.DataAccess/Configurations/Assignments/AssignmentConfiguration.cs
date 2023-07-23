using ITMO.Dev.ASAP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Assignments;

public class AssignmentConfiguration : IEntityTypeConfiguration<AssignmentModel>
{
    public void Configure(EntityTypeBuilder<AssignmentModel> builder)
    {
        builder
            .HasMany(x => x.GroupAssignments)
            .WithOne(x => x.Assignment)
            .HasForeignKey(x => x.AssignmentId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasOne(x => x.SubjectCourse)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasIndex(nameof(AssignmentModel.ShortName), nameof(AssignmentModel.SubjectCourseId))
            .IsUnique();
    }
}