using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class RenamedDeadlinePolicyToDeadlinePenalties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                update "DeadlinePolicy"
                set "Discriminator" =
                    case "Discriminator"
                    when 'AbsoluteDeadlinePolicy' then 'AbsoluteDeadlinePenalty'
                    when 'FractionDeadlinePolicy' then 'FractionDeadlinePenalty'
                    when 'CappingDeadlinePolicy' then 'CappingDeadlinePenalty'
                    else 'DeadlinePenalty'
                    end
            """);

            migrationBuilder.RenameTable(
                name: "DeadlinePolicy",
                newName: "DeadlinePenalties");

            migrationBuilder.CreateTable(
                name: "DeadlinePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_DeadlinePolicies", x => x.Id));

            migrationBuilder.AddColumn<Guid>(
                table: "DeadlinePenalties",
                name: "DeadlinePolicyId",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeadlinePenalties_DeadlinePolicies_DeadlinePolicyId",
                table: "DeadlinePenalties",
                column: "DeadlinePolicyId",
                principalTable: "DeadlinePolicies",
                principalColumn: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DeadlinePenalties_DeadlinePolicyId",
                table: "DeadlinePenalties",
                column: "DeadlinePolicyId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeadlinePolicy",
                table: "DeadlinePenalties");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeadlinePenalties",
                column: "Id",
                table: "DeadlinePenalties");

            migrationBuilder.RenameIndex(
                name: "IX_DeadlinePolicy_SubjectCourseId",
                newName: "IX_DeadlinePenalties_SubjectCourseId",
                table: "DeadlinePenalties");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_DeadlinePenalties_SubjectCourseId",
                newName: "IX_DeadlinePolicy_SubjectCourseId",
                table: "DeadlinePenalties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeadlinePenalties",
                table: "DeadlinePenalties");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeadlinePolicy",
                column: "Id",
                table: "DeadlinePenalties");

            migrationBuilder.DropIndex(
                name: "IX_DeadlinePenalties_DeadlinePolicyId",
                table: "DeadlinePenalties");

            migrationBuilder.DropForeignKey(
                name: "FK_DeadlinePenalties_DeadlinePolicies_DeadlinePolicyId",
                table: "DeadlinePenalties");

            migrationBuilder.DropColumn(
                table: "DeadlinePenalties",
                name: "DeadlinePolicyId");

            migrationBuilder.DropTable(name: "DeadlinePolicies");

            migrationBuilder.RenameTable(
                name: "DeadlinePenalties",
                newName: "DeadlinePolicy");

            migrationBuilder.Sql("""
                update "DeadlinePolicy"
                set "Discriminator" =
                    case "Discriminator"
                    when 'AbsoluteDeadlinePenalty' then 'AbsoluteDeadlinePolicy'
                    when 'FractionDeadlinePenalty' then 'FractionDeadlinePolicy'
                    when 'CappingDeadlinePenalty' then 'CappingDeadlinePolicy'
                    else 'DeadlinePolicy'
                    end
            """);
        }
    }
}
