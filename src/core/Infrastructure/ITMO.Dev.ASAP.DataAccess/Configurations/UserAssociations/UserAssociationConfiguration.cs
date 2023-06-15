using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.UserAssociations;

public class UserAssociationConfiguration : IEntityTypeConfiguration<UserAssociationModel>
{
    private const string DiscriminatorColumnName = "Discriminator";

    public void Configure(EntityTypeBuilder<UserAssociationModel> builder)
    {
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Associations)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id);

        builder.HasDiscriminator<string>(DiscriminatorColumnName)
            .HasValue<IsuUserAssociationModel>(nameof(IsuUserAssociation));

        builder
            .HasIndex(nameof(UserAssociationModel.UserId), DiscriminatorColumnName)
            .IsUnique();
    }
}