using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePolicies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class DeadlinePolicyConfiguration : IEntityTypeConfiguration<DeadlinePolicy>
{
    public void Configure(EntityTypeBuilder<DeadlinePolicy> builder)
    {
        builder.ToTable("DeadlinePolicies");
        builder.HasMany(dp => dp.DeadlinePenalties).WithOne();

        builder
            .Navigation(dp => dp.DeadlinePenalties)
            .HasField("_deadlinePenalties");
    }
}