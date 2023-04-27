using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Submissions;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasOne(x => x.Student);
        builder.HasOne(x => x.GroupAssignment);
    }
}