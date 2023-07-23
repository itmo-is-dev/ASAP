using ITMO.Dev.ASAP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.SubjectCourses;

public class SubjectCourseConfiguration : IEntityTypeConfiguration<SubjectCourseModel>
{
    public void Configure(EntityTypeBuilder<SubjectCourseModel> builder)
    {
        builder
            .HasOne(x => x.Subject)
            .WithMany(x => x.SubjectCourses)
            .HasForeignKey(x => x.SubjectId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.DeadlinePenalties)
            .WithOne(x => x.SubjectCourse)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.Mentors)
            .WithOne(x => x.SubjectCourse)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.SubjectCourseGroups)
            .WithOne(x => x.SubjectCourse)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.Assignments)
            .WithOne(x => x.SubjectCourse)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);
    }
}