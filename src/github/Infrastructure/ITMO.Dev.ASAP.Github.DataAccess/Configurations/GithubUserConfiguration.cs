using ITMO.Dev.ASAP.Github.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.Github.DataAccess.Configurations;

public class GithubUserConfiguration : IEntityTypeConfiguration<GithubUser>
{
    public void Configure(EntityTypeBuilder<GithubUser> builder)
    {
        builder.ToTable("GithubUsers");
    }
}