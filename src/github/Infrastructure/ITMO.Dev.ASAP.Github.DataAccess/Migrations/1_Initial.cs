using FluentMigrator;

namespace ITMO.Dev.ASAP.Github.DataAccess.Migrations;

#pragma warning disable SA1649

[Migration(1, "Initial")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services)
    {
        return """
        create table if not exists "GithubAssignments"
        (
            "Id" uuid not null primary key,
            "SubjectCourseId" uuid not null,
            "BranchName" text not null
        );

        create table if not exists "GithubSubjectCourses"
        (
            "Id" uuid not null primary key,
            "OrganizationName" text not null,
            "TemplateRepositoryName" text not null,
            "MentorTeamName" text not null
        );

        create table if not exists "GithubSubmissions"
        (
            "Id" uuid not null primary key,
            "AssignmentId" uuid not null,
            "UserId" uuid not null,
            "CreatedAt" timestamp with time zone not null,
            "Organization" text not null,
            "Repository" text not null,
            "PullRequestNumber" bigint not null
        );
        
        create table if not exists "GithubUsers"
        (
            "Id" uuid not null primary key,
            "Username" text not null
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider services)
    {
        return """
        drop table if exists "GithubAssignments";
        drop table if exists "GithubSubjectCourses";
        drop table if exists "GithubSubmissions";
        drop table if exists "GithubUsers";
        """;
    }
}