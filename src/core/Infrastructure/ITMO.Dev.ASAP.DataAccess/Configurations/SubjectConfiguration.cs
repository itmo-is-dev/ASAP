using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.Navigation(x => x.Courses).HasField("_courses");
    }
}