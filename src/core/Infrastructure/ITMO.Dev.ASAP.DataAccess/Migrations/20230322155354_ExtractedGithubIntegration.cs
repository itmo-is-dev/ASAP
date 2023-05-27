using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class ExtractedGithubIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            do $$
                begin 
                    if exists
                    (
                        (SELECT 1
                         FROM information_schema.tables 
                         WHERE table_schema = 'public'
                           AND table_name = 'GithubSubmissions')
                    )
                    then 
                        insert into "GithubSubmissions"
                        ("Id", "AssignmentId", "UserId", "CreatedAt", "Organization", "Repository", "PullRequestNumber")
                        select "SubmissionId", "GroupAssignmentAssignmentId", "StudentUserId", "SubmissionDate", "Organization", "Repository", "PrNumber"
                        from "SubmissionAssociations" as sa
                        join "Submissions" S on S."Id" = sa."SubmissionId"
                        where sa."Discriminator" = 'GithubSubmissionAssociation';
                    end if;
                end;
            $$
            """);

            migrationBuilder.Sql("""
            do $$
                begin 
                    if exists
                    (
                        (SELECT 1
                         FROM information_schema.tables 
                         WHERE table_schema = 'public'
                           AND table_name = 'GithubUsers')
                    )
                    then 
                        insert into "GithubUsers" as gu
                        ("Id", "Username")
                        select ua."UserId", ua."GithubUsername"
                        from "UserAssociations" as ua
                        where ua."Discriminator" = 'GithubUserAssociation';
                    end if;
                end;
            $$
            """);

            migrationBuilder.Sql("""
            do $$
                begin 
                    if exists
                    (
                        (SELECT 1
                         FROM information_schema.tables 
                         WHERE table_schema = 'public'
                           AND table_name = 'GithubSubjectCourses')
                    )
                    then 
                        insert into "GithubSubjectCourses"
                        ("Id", "OrganizationName", "TemplateRepositoryName", "MentorTeamName")
                        select "SubjectCourseId", "GithubOrganizationName", "TemplateRepositoryName", coalesce("MentorTeamName", '')
                        from "SubjectCourseAssociations"
                        where "Discriminator" = 'GithubSubjectCourseAssociation';
                    end if;
                end;
            $$
            """);

            migrationBuilder.Sql("""
            do $$
                begin 
                    if exists
                    (
                        (SELECT 1
                         FROM information_schema.tables 
                         WHERE table_schema = 'public'
                           AND table_name = 'GithubAssignments')
                    )
                    then 
                        insert into "GithubAssignments"
                        ("Id", "BranchName", "SubjectCourseId")
                        select "Id", "ShortName", "SubjectCourseId"
                        from "Assignments";
                    end if;
                end;
            $$
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
