using ITMO.Dev.ASAP.Github.Domain.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.Github.DataAccess.Configurations;

public class GithubAssignmentConfiguration : IEntityTypeConfiguration<GithubAssignment>
{
    public void Configure(EntityTypeBuilder<GithubAssignment> builder)
    {
        builder.ToTable("GithubAssignments");
    }
}