using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IsuUserAssociation = ITMO.Dev.ASAP.Domain.UserAssociations.IsuUserAssociation;
using UserAssociation = ITMO.Dev.ASAP.Domain.UserAssociations.UserAssociation;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.UserAssociations;

public class UserAssociationConfiguration : IEntityTypeConfiguration<UserAssociation>
{
    public void Configure(EntityTypeBuilder<UserAssociation> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<IsuUserAssociation>(nameof(IsuUserAssociation));

        builder.HasIndex("UserId", "Discriminator").IsUnique();
    }
}