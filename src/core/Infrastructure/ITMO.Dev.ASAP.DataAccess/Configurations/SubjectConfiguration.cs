using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<SubjectModel>
{
    public void Configure(EntityTypeBuilder<SubjectModel> builder)
    {
        builder
            .HasMany<SubjectCourse>()
            .WithOne()
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.SubjectId);

        builder.ToTable("Subjects");
    }
}