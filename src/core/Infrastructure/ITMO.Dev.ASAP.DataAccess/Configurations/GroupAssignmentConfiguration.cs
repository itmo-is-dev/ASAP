using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GroupAssignment = ITMO.Dev.ASAP.Domain.Study.GroupAssignment;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class GroupAssignmentConfiguration : IEntityTypeConfiguration<GroupAssignment>
{
    public void Configure(EntityTypeBuilder<GroupAssignment> builder)
    {
        builder.HasKey(x => new { x.GroupId, x.AssignmentId });

        builder.Navigation(x => x.Submissions).HasField("_submissions");
    }
}