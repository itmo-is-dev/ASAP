using Dapper;
using FluentAssertions;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.DataAccess;
using ITMO.Dev.ASAP.Github.DataAccess.Models;
using ITMO.Dev.ASAP.Github.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Tests.Github.Extensions;
using ITMO.Dev.ASAP.Tests.Github.Fixtures;
using ITMO.Dev.ASAP.Tests.Github.Tools;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Github.Repositories;

[Collection(nameof(DatabaseCollectionFixture))]
public class GithubAssignmentRepositoryTests : IAsyncLifetime
{
    private readonly GithubDatabaseFixture _database;
    private readonly DeterministicFaker _faker;

    public GithubAssignmentRepositoryTests(GithubDatabaseFixture database, DeterministicFaker faker)
    {
        _database = database;
        _faker = faker;
    }

    [Fact]
    public async Task Add_ShouldAddDatabaseRecordCorrectly()
    {
        // Arrange
        using var unit = new UnitOfWork(_database.GithubConnection);
        var repository = new GithubAssignmentRepository(_database.GithubConnection, unit);
        GithubAssignment assignment = _faker.GithubAssignment();

        // Act
        repository.Add(assignment);
        await unit.CommitAsync(default);

        // Assert
        const string sql = """
        select "Id", "SubjectCourseId", "BranchName" from "GithubAssignments"
        """;

        GithubAssignmentModel[] assignments = _database.Connection.Query<GithubAssignmentModel>(sql).ToArray();

        GithubAssignmentModel assignmentModel = assignments.Should().ContainSingle().Subject;
        assignmentModel.Should().NotBeEquivalentTo(assignment);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnCorrectRecords()
    {
        // Arrange
        using var unit = new UnitOfWork(_database.GithubConnection);
        var repository = new GithubAssignmentRepository(_database.GithubConnection, unit);

        Guid subjectCourseId = _faker.Random.Guid();
        Guid subjectCourseId2 = _faker.Random.Guid();
        Guid subjectCourseId3 = _faker.Random.Guid();
        string subjectCourseName = _faker.Company.CompanyName();

        GithubSubjectCourseModel args = _faker.GithubSubjectCourseModel(id: subjectCourseId);
        GithubSubjectCourseModel args2 = args with { Id = subjectCourseId2 };
        GithubSubjectCourseModel args3 = args with { Id = subjectCourseId3, OrganizationName = subjectCourseName };

        await _database.Connection.ExecuteAsync(GithubSubjectCourseRepository.AddSql, args);
        await _database.Connection.ExecuteAsync(GithubSubjectCourseRepository.AddSql, args2);
        await _database.Connection.ExecuteAsync(GithubSubjectCourseRepository.AddSql, args3);

        GithubAssignment[] seedAssignments =
        {
            _faker.GithubAssignment(subjectCourseId: subjectCourseId),
            _faker.GithubAssignment(subjectCourseId: subjectCourseId2),
            _faker.GithubAssignment(subjectCourseId: subjectCourseId),
            _faker.GithubAssignment(subjectCourseId: subjectCourseId3),
        };

        foreach (GithubAssignment assignment in seedAssignments)
        {
            repository.Add(assignment);
        }

        await unit.CommitAsync(default);

        var query1 = GithubAssignmentQuery.Build(x => x.WithId(seedAssignments[0].Id));
        var query2 = GithubAssignmentQuery.Build(x => x.WithSubjectCourseId(subjectCourseId2));
        var query3 = GithubAssignmentQuery.Build(x => x.WithBranchName(seedAssignments[2].BranchName));
        var query4 = GithubAssignmentQuery.Build(x => x.WithSubjectCourseOrganizationName(subjectCourseName));

        // Act
        GithubAssignment assignment1 = await repository.QueryAsync(query1, default).SingleAsync();
        GithubAssignment assignment2 = await repository.QueryAsync(query2, default).SingleAsync();
        GithubAssignment assignment3 = await repository.QueryAsync(query3, default).SingleAsync();
        GithubAssignment assignment4 = await repository.QueryAsync(query4, default).SingleAsync();

        // Assert
        assignment1.Should().BeEquivalentTo(seedAssignments[0]);
        assignment2.Should().BeEquivalentTo(seedAssignments[1]);
        assignment3.Should().BeEquivalentTo(seedAssignments[2]);
        assignment4.Should().BeEquivalentTo(seedAssignments[3]);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _database.ResetAsync();
    }
}