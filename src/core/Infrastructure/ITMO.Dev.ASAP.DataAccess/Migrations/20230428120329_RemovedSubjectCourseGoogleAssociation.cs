using Microsoft.EntityFrameworkCore.Migrations;

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class RemovedSubjectCourseGoogleAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoogleSubjectCourse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpreadsheetId = table.Column<string>("text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleSubjectCourse", x => x.Id);
                });

            migrationBuilder.Sql("""
            insert into "GoogleSubjectCourse" ("Id", "SpreadsheetId")
            select "SubjectCourseId", "SpreadsheetId" from "SubjectCourseAssociations" as sca
            where sca."Discriminator" = 'GoogleTableSubjectCourseAssociation'
            """);

            migrationBuilder.DropTable(
                name: "SubjectCourseAssociations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectCourseAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    SpreadsheetId = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCourseAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectCourseAssociations_SubjectCourses_SubjectCourseId",
                        column: x => x.SubjectCourseId,
                        principalTable: "SubjectCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourseAssociations_SubjectCourseId_Discriminator",
                table: "SubjectCourseAssociations",
                columns: new[] { "SubjectCourseId", "Discriminator" },
                unique: true);

            migrationBuilder.Sql("""
            insert into "SubjectCourseAssociations" ("SubjectCourseId", "SpreadsheetId", "Discriminator")
            select "Id", "SpreadsheetId", 'GoogleTableSubjectCourseAssociation' from "GoogleSubjectCourse" as sca
            """);

            migrationBuilder.DropTable(name: "GoogleSubjectCourse");
        }
    }
}
