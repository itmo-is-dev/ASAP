using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.Github.DataAccess.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GithubAssignments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SubjectCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                BranchName = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GithubAssignments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GithubSubjectCourses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrganizationName = table.Column<string>(type: "text", nullable: false),
                TemplateRepositoryName = table.Column<string>(type: "text", nullable: false),
                MentorTeamName = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GithubSubjectCourses", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GithubSubmissions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Organization = table.Column<string>(type: "text", nullable: false),
                Repository = table.Column<string>(type: "text", nullable: false),
                PullRequestNumber = table.Column<long>(type: "bigint", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GithubSubmissions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GithubUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Username = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GithubUsers", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GithubAssignments");

        migrationBuilder.DropTable(
            name: "GithubSubjectCourses");

        migrationBuilder.DropTable(
            name: "GithubSubmissions");

        migrationBuilder.DropTable(
            name: "GithubUsers");
    }
}