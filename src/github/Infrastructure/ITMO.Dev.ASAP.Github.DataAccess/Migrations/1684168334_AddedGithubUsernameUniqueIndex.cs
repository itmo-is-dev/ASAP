using FluentMigrator;

namespace ITMO.Dev.ASAP.Github.DataAccess.Migrations;

#pragma warning disable SA1649

[Migration(1684168334, "Added GithubUsers.Username unique index")]
public class AddedGithubUsernameUniqueIndex : SqlMigration
{
    public const string IndexName = "github_user_username";

    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
        create unique index {IndexName}
        on "GithubUsers"("Username")
        """;
    }

    protected override string GetDownSql(IServiceProvider services)
    {
        return $"""
        drop index {IndexName}
        """;
    }
}