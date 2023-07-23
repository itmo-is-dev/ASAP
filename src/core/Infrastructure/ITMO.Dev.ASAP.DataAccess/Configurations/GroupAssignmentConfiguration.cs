using ITMO.Dev.ASAP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class GroupAssignmentConfiguration : IEntityTypeConfiguration<GroupAssignmentModel>
{
    public void Configure(EntityTypeBuilder<GroupAssignmentModel> builder)
    {
        builder.HasKey(x => new { x.StudentGroupId, x.AssignmentId });

        builder
            .HasMany(x => x.Submissions)
            .WithOne(x => x.GroupAssignment)
            .HasForeignKey(x => new { x.StudentGroupId, x.AssignmentId })
            .HasPrincipalKey(x => new { x.StudentGroupId, x.AssignmentId });
    }
}