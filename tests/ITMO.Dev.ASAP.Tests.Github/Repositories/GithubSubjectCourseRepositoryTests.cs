using Dapper;
using FluentAssertions;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.DataAccess;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Tests.Github.Extensions;
using ITMO.Dev.ASAP.Tests.Github.Fixtures;
using ITMO.Dev.ASAP.Tests.Github.Tools;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Github.Repositories;

[Collection(nameof(DatabaseCollectionFixture))]
public class GithubSubjectCourseRepositoryTests : IAsyncLifetime
{
    private readonly DatabaseFixture _database;
    private readonly DeterministicFaker _faker;

    public GithubSubjectCourseRepositoryTests(DatabaseFixture database, DeterministicFaker faker)
    {
        _database = database;
        _faker = faker;
    }

    [Fact]
    public async Task Add_ShouldAddDatabaseRecordCorrectly()
    {
        // Arrange
        using var unit = new UnitOfWork(_database.Connection);
        var repository = new GithubSubjectCourseRepository(_database.Connection, unit);
        GithubSubjectCourse subjectCourse = _faker.GithubSubjectCourse();

        // Act
        repository.Add(subjectCourse);
        await unit.CommitAsync(default);

        // Assert
        const string sql = """
        select "Id", "OrganizationName", "TemplateRepositoryName", "MentorTeamName" from "GithubSubjectCourses"
        """;

        GithubSubjectCourseModel[] subjectCourses = _database.RawConnection.Query<GithubSubjectCourseModel>(sql).ToArray();

        GithubSubjectCourseModel subjectCourseModel = subjectCourses.Should().ContainSingle().Subject;
        subjectCourseModel.Should().NotBeEquivalentTo(subjectCourse);
    }

    [Fact]
    public async Task Update_ShouldUpdateDatabaseRecordCorrectly()
    {
        // Arrange
        using var unit = new UnitOfWork(_database.Connection);
        var repository = new GithubSubjectCourseRepository(_database.Connection, unit);
        GithubSubjectCourse subjectCourse = _faker.GithubSubjectCourse();

        repository.Add(subjectCourse);
        await unit.CommitAsync(default);

        subjectCourse.MentorTeamName = _faker.Company.CompanySuffix();

        // Act
        repository.Update(subjectCourse);
        await unit.CommitAsync(default);

        // Assert
        const string sql = """
        select "Id", "OrganizationName", "TemplateRepositoryName", "MentorTeamName" from "GithubSubjectCourses"
        """;

        GithubSubjectCourseModel[] subjectCourses = _database.RawConnection.Query<GithubSubjectCourseModel>(sql).ToArray();

        GithubSubjectCourseModel subjectCourseModel = subjectCourses.Should().ContainSingle().Subject;
        subjectCourseModel.Should().NotBeEquivalentTo(subjectCourse);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnCorrectRecords()
    {
        // Arrange
        using var unit = new UnitOfWork(_database.Connection);
        var repository = new GithubSubjectCourseRepository(_database.Connection, unit);

        GithubSubjectCourse[] courses =
        {
            _faker.GithubSubjectCourse(),
            _faker.GithubSubjectCourse(),
            _faker.GithubSubjectCourse(),
            _faker.GithubSubjectCourse(),
        };

        foreach (GithubSubjectCourse course in courses)
        {
            repository.Add(course);
        }

        await unit.CommitAsync(default);

        var query1 = GithubSubjectCourseQuery.Build(x => x.WithId(courses[0].Id));
        var query2 = GithubSubjectCourseQuery.Build(x => x.WithOrganizationName(courses[1].OrganizationName));
        var query3 = GithubSubjectCourseQuery.Build(x => x.WithTemplateRepositoryName(courses[2].TemplateRepositoryName));
        var query4 = GithubSubjectCourseQuery.Build(x => x.WithMentorTeamName(courses[3].MentorTeamName));

        // Act
        GithubSubjectCourse course1 = await repository.QueryAsync(query1, default).SingleAsync();
        GithubSubjectCourse course2 = await repository.QueryAsync(query2, default).SingleAsync();
        GithubSubjectCourse course3 = await repository.QueryAsync(query3, default).SingleAsync();
        GithubSubjectCourse course4 = await repository.QueryAsync(query4, default).SingleAsync();

        // Assert
        course1.Should().BeEquivalentTo(courses[0]);
        course2.Should().BeEquivalentTo(courses[1]);
        course3.Should().BeEquivalentTo(courses[2]);
        course4.Should().BeEquivalentTo(courses[3]);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}