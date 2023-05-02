using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class DeadlinePolicyConfiguration : IEntityTypeConfiguration<DeadlinePolicy>
{
    public void Configure(EntityTypeBuilder<DeadlinePolicy> builder)
    {
        builder
            .Navigation(dp => dp.DeadlinePenalties)
            .HasField("_deadlinePenalties");
    }
}