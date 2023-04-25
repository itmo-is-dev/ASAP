using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class DeadlinePenaltyConfiguration : IEntityTypeConfiguration<DeadlinePenalty>
{
    public void Configure(EntityTypeBuilder<DeadlinePenalty> builder)
    {
        builder.Property<Guid>("Id");
        builder.HasKey("Id");

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<AbsoluteDeadlinePenalty>(nameof(AbsoluteDeadlinePenalty))
            .HasValue<FractionDeadlinePenalty>(nameof(FractionDeadlinePenalty))
            .HasValue<CappingDeadlinePenalty>(nameof(CappingDeadlinePenalty));
    }
}