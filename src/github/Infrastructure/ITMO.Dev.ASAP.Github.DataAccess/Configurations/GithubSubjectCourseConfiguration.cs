using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.Github.DataAccess.Configurations;

public class GithubSubjectCourseConfiguration : IEntityTypeConfiguration<GithubSubjectCourse>
{
    public void Configure(EntityTypeBuilder<GithubSubjectCourse> builder)
    {
        builder.ToTable("GithubSubjectCourses");
    }
}