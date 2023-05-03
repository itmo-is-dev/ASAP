using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    public partial class RenamedDeadlinePenaltiesDiscriminator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                update "DeadlinePenalties"
                set "Discriminator" =
                    case "Discriminator"
                    when 'AbsoluteDeadlinePolicy' then 'AbsoluteDeadlinePenalty'
                    when 'FractionDeadlinePolicy' then 'FractionDeadlinePenalty'
                    when 'CappingDeadlinePolicy' then 'CappingDeadlinePenalty'
                    else 'DeadlinePenalty'
                    end
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                update "DeadlinePenalties"
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
