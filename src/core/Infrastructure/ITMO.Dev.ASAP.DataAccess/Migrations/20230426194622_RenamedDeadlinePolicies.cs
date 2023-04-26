using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class RenamedDeadlinePolicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DeadlinePolicy",
                newName: "DeadlinePenalty");

            migrationBuilder.CreateTable(
                name: "DeadlinePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_DeadlinePolicies", x => x.Id));

            migrationBuilder.AddColumn<Guid>(
                table: "DeadlinePenalty",
                name: "DeadlinePolicyId",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeadlinePenalty_DeadlinePolicies_DeadlinePolicyId",
                table: "DeadlinePenalty",
                column: "DeadlinePolicyId",
                principalTable: "DeadlinePolicies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeadlinePenalty_DeadlinePolicies_DeadlinePolicyId",
                table: "DeadlinePenalty");

            migrationBuilder.DropColumn(
                table: "DeadlinePenalty",
                name: "DeadlinePolicyId");

            migrationBuilder.DropTable(name: "DeadlinePolicies");

            migrationBuilder.RenameTable(
                name: "DeadlinePenalty",
                newName: "DeadlinePolicy");
        }
    }
}
