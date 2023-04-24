using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class RenamedDeadlinePolicyToDeadlinePenalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DeadlinePolicy",
                newName: "DeadlinePenalty");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DeadlinePenalty",
                newName: "DeadlinePolicy");
        }
    }
}
