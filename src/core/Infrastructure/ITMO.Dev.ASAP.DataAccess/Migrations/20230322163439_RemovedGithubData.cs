using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class RemovedGithubData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            delete from "UserAssociations"
            where "Discriminator" = 'GithubUserAssociation'
            """);

            migrationBuilder.Sql("""
            delete from "SubjectCourseAssociations"
            where "Discriminator" = 'GithubSubjectCourseAssociation'
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
