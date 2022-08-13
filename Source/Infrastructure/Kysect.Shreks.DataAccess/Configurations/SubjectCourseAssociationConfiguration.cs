using Kysect.Shreks.Core.SubjectCourseAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class SubjectCourseAssociationConfiguration : IEntityTypeConfiguration<SubjectCourseAssociation>
{
    public void Configure(EntityTypeBuilder<SubjectCourseAssociation> builder)
    {
        builder.HasDiscriminator<string>("SubjectCourseAssociationType")
            .HasValue<GithubSubjectCourseAssociation>(nameof(GithubSubjectCourseAssociation));
    }
}