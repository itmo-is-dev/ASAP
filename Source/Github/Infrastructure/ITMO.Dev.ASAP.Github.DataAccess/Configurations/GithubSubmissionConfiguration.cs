using ITMO.Dev.ASAP.Github.Domain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.Github.DataAccess.Configurations;

public class GithubSubmissionConfiguration : IEntityTypeConfiguration<GithubSubmission>
{
    public void Configure(EntityTypeBuilder<GithubSubmission> builder)
    {
        builder.ToTable("GithubSubmissions");
    }
}