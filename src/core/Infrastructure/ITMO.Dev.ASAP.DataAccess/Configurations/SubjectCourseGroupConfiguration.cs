using ITMO.Dev.ASAP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class SubjectCourseGroupConfiguration : IEntityTypeConfiguration<SubjectCourseGroupModel>
{
    public void Configure(EntityTypeBuilder<SubjectCourseGroupModel> builder)
    {
        builder.HasKey(x => new { x.SubjectCourseId, x.StudentGroupId });

        builder
            .HasOne(x => x.SubjectCourse)
            .WithMany(x => x.SubjectCourseGroups)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasOne(x => x.StudentGroup)
            .WithMany(x => x.SubjectCourseGroups)
            .HasForeignKey(x => x.StudentGroupId)
            .HasPrincipalKey(x => x.Id);
    }
}