using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.SubjectCourses;

public class SubjectCourseAssociationConfiguration : IEntityTypeConfiguration<SubjectCourseAssociation>
{
    public void Configure(EntityTypeBuilder<SubjectCourseAssociation> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<GoogleTableSubjectCourseAssociation>(nameof(GoogleTableSubjectCourseAssociation));

        builder.HasIndex("SubjectCourseId", "Discriminator").IsUnique();
    }
}