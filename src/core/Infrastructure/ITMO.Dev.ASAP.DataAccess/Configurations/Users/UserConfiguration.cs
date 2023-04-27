using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User = ITMO.Dev.ASAP.Domain.Users.User;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Navigation(x => x.Associations).HasField("_associations");
    }
}