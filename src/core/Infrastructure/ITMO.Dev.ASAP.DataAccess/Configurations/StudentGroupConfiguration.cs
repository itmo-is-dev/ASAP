using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentGroup = ITMO.Dev.ASAP.Domain.Study.StudentGroup;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.Navigation(x => x.Students).HasField("_students");
    }
}