using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IsuUserAssociation = ITMO.Dev.ASAP.Domain.UserAssociations.IsuUserAssociation;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.UserAssociations;

public class IsuUserAssociationConfiguration : IEntityTypeConfiguration<IsuUserAssociation>
{
    public void Configure(EntityTypeBuilder<IsuUserAssociation> builder)
    {
        builder.HasIndex(x => x.UniversityId).IsUnique();
    }
}