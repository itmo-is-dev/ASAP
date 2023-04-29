using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DeadlinePolicy = ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies.DeadlinePolicy;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class DeadlinePolicyConfiguration : IEntityTypeConfiguration<DeadlinePolicy>
{
    public void Configure(EntityTypeBuilder<DeadlinePolicy> builder)
    {
        builder.HasMany(dp => dp.DeadlinePenalties).WithOne();

        builder
            .Navigation(dp => dp.DeadlinePenalties)
            .HasField("_deadlinePenalties");
    }
}