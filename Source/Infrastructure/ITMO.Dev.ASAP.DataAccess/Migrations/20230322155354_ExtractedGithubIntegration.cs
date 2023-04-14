using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class ExtractedGithubIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            insert into "GithubSubmissions"
            ("Id", "AssignmentId", "UserId", "CreatedAt", "Organization", "Repository", "PullRequestNumber")
            select "SubmissionId", "GroupAssignmentAssignmentId", "StudentUserId", "SubmissionDate", "Organization", "Repository", "PrNumber"
            from "SubmissionAssociations"
            join "Submissions" S on S."Id" = "SubmissionAssociations"."SubmissionId"
            where "Discriminator" = 'GithubSubmissionAssociation'
            """);

            migrationBuilder.Sql("""
            insert into "GithubUsers"
            ("Id", "Username")
            select "UserId", "GithubUsername"
            from "UserAssociations"
            where "Discriminator" = 'GithubUserAssociation'
            """);

            migrationBuilder.Sql("""
            insert into "GithubSubjectCourses"
            ("Id", "OrganizationName", "TemplateRepositoryName", "MentorTeamName")
            select "SubjectCourseId", "GithubOrganizationName", "TemplateRepositoryName", "MentorTeamName"
            from "SubjectCourseAssociations"
            where "Discriminator" = 'GithubSubjectCourseAssociation'
            """);

            migrationBuilder.Sql("""
            insert into "GithubAssignments"
            ("Id", "BranchName", "SubjectCourseId")
            select "Id", "ShortName", "SubjectCourseId"
            from "Assignments"
            """);

            migrationBuilder.DropTable(
                name: "SubmissionAssociations");

            migrationBuilder.DropIndex(
                name: "IX_UserAssociations_GithubUsername",
                table: "UserAssociations");

            migrationBuilder.DropIndex(
                name: "IX_SubjectCourseAssociations_GithubOrganizationName",
                table: "SubjectCourseAssociations");

            migrationBuilder.DropColumn(
                name: "GithubUsername",
                table: "UserAssociations");

            migrationBuilder.DropColumn(
                name: "GithubOrganizationName",
                table: "SubjectCourseAssociations");

            migrationBuilder.DropColumn(
                name: "MentorTeamName",
                table: "SubjectCourseAssociations");

            migrationBuilder.DropColumn(
                name: "TemplateRepositoryName",
                table: "SubjectCourseAssociations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GithubUsername",
                table: "UserAssociations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GithubOrganizationName",
                table: "SubjectCourseAssociations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MentorTeamName",
                table: "SubjectCourseAssociations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateRepositoryName",
                table: "SubjectCourseAssociations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubmissionAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Organization = table.Column<string>(type: "text", nullable: true),
                    PrNumber = table.Column<long>(type: "bigint", nullable: true),
                    Repository = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionAssociations_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAssociations_GithubUsername",
                table: "UserAssociations",
                column: "GithubUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourseAssociations_GithubOrganizationName",
                table: "SubjectCourseAssociations",
                column: "GithubOrganizationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAssociations_SubmissionId_Discriminator",
                table: "SubmissionAssociations",
                columns: new[] { "SubmissionId", "Discriminator" },
                unique: true);
        }
    }
}
