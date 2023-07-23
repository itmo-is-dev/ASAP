using ITMO.Dev.ASAP.DataAccess.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Users;

public class MentorConfiguration : IEntityTypeConfiguration<MentorModel>
{
    public void Configure(EntityTypeBuilder<MentorModel> builder)
    {
        builder.HasKey(x => new { x.UserId, CourseId = x.SubjectCourseId });

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Mentors)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasOne(x => x.SubjectCourse)
            .WithMany(x => x.Mentors)
            .HasForeignKey(x => x.SubjectCourseId)
            .HasPrincipalKey(x => x.Id);
    }
}