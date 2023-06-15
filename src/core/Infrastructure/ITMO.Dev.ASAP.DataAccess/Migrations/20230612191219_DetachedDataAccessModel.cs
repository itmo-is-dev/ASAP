using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class DetachedDataAccessModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeadlinePenalties_DeadlinePolicies_DeadlinePolicyId",
                table: "DeadlinePenalties");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssignments_StudentGroups_GroupId",
                table: "GroupAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentors_SubjectCourses_CourseId",
                table: "Mentors");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_GroupId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_GroupAssignments_GroupAssignmentGroupId_GroupAs~",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Students_StudentUserId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "DeadlinePolicies");

            migrationBuilder.DropIndex(
                name: "IX_DeadlinePenalties_DeadlinePolicyId",
                table: "DeadlinePenalties");

            migrationBuilder.DropColumn(
                name: "DeadlinePolicyId",
                table: "DeadlinePenalties");

            migrationBuilder.RenameColumn(
                name: "StudentUserId",
                table: "Submissions",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "GroupAssignmentGroupId",
                table: "Submissions",
                newName: "StudentGroupId");

            migrationBuilder.RenameColumn(
                name: "GroupAssignmentAssignmentId",
                table: "Submissions",
                newName: "AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_StudentUserId",
                table: "Submissions",
                newName: "IX_Submissions_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_GroupAssignmentGroupId_GroupAssignmentAssignmen~",
                table: "Submissions",
                newName: "IX_Submissions_StudentGroupId_AssignmentId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Students",
                newName: "StudentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_GroupId",
                table: "Students",
                newName: "IX_Students_StudentGroupId");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Mentors",
                newName: "SubjectCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Mentors_CourseId",
                table: "Mentors",
                newName: "IX_Mentors_SubjectCourseId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "GroupAssignments",
                newName: "StudentGroupId");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectCourseId",
                table: "DeadlinePenalties",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssignments_StudentGroups_StudentGroupId",
                table: "GroupAssignments",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentors_SubjectCourses_SubjectCourseId",
                table: "Mentors",
                column: "SubjectCourseId",
                principalTable: "SubjectCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_StudentGroupId",
                table: "Students",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_GroupAssignments_StudentGroupId_AssignmentId",
                table: "Submissions",
                columns: new[] { "StudentGroupId", "AssignmentId" },
                principalTable: "GroupAssignments",
                principalColumns: new[] { "StudentGroupId", "AssignmentId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Students_StudentId",
                table: "Submissions",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssignments_StudentGroups_StudentGroupId",
                table: "GroupAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentors_SubjectCourses_SubjectCourseId",
                table: "Mentors");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_StudentGroupId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_GroupAssignments_StudentGroupId_AssignmentId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Students_StudentId",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Submissions",
                newName: "StudentUserId");

            migrationBuilder.RenameColumn(
                name: "StudentGroupId",
                table: "Submissions",
                newName: "GroupAssignmentGroupId");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "Submissions",
                newName: "GroupAssignmentAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_StudentId",
                table: "Submissions",
                newName: "IX_Submissions_StudentUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_StudentGroupId_AssignmentId",
                table: "Submissions",
                newName: "IX_Submissions_GroupAssignmentGroupId_GroupAssignmentAssignmen~");

            migrationBuilder.RenameColumn(
                name: "StudentGroupId",
                table: "Students",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_StudentGroupId",
                table: "Students",
                newName: "IX_Students_GroupId");

            migrationBuilder.RenameColumn(
                name: "SubjectCourseId",
                table: "Mentors",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Mentors_SubjectCourseId",
                table: "Mentors",
                newName: "IX_Mentors_CourseId");

            migrationBuilder.RenameColumn(
                name: "StudentGroupId",
                table: "GroupAssignments",
                newName: "GroupId");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectCourseId",
                table: "DeadlinePenalties",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "DeadlinePolicyId",
                table: "DeadlinePenalties",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeadlinePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeadlinePolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeadlinePenalties_DeadlinePolicyId",
                table: "DeadlinePenalties",
                column: "DeadlinePolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeadlinePenalties_DeadlinePolicies_DeadlinePolicyId",
                table: "DeadlinePenalties",
                column: "DeadlinePolicyId",
                principalTable: "DeadlinePolicies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssignments_StudentGroups_GroupId",
                table: "GroupAssignments",
                column: "GroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentors_SubjectCourses_CourseId",
                table: "Mentors",
                column: "CourseId",
                principalTable: "SubjectCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_GroupId",
                table: "Students",
                column: "GroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_GroupAssignments_GroupAssignmentGroupId_GroupAs~",
                table: "Submissions",
                columns: new[] { "GroupAssignmentGroupId", "GroupAssignmentAssignmentId" },
                principalTable: "GroupAssignments",
                principalColumns: new[] { "GroupId", "AssignmentId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Students_StudentUserId",
                table: "Submissions",
                column: "StudentUserId",
                principalTable: "Students",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
