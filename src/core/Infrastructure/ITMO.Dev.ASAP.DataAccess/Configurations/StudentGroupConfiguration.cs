using ITMO.Dev.ASAP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroupModel>
{
    public void Configure(EntityTypeBuilder<StudentGroupModel> builder)
    {
        builder
            .HasMany(x => x.Students)
            .WithOne(x => x.StudentGroup)
            .HasForeignKey(x => x.StudentGroupId)
            .HasPrincipalKey(x => x.Id);
    }
}