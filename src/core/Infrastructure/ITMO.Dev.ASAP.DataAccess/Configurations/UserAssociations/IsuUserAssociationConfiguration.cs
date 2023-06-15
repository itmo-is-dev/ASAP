using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.UserAssociations;

public class IsuUserAssociationConfiguration : IEntityTypeConfiguration<IsuUserAssociationModel>
{
    public void Configure(EntityTypeBuilder<IsuUserAssociationModel> builder)
    {
        builder.HasIndex(x => x.UniversityId).IsUnique();
    }
}