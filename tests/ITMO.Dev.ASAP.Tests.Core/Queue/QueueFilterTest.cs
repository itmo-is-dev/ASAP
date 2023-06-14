using FluentAssertions;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Queue;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Queue;
using ITMO.Dev.ASAP.Domain.Queue.Building;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Queue;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class QueueFilterTest : TestBase, IAsyncDisposeLifetime
{
    private readonly CoreDatabaseFixture _database;

    public QueueFilterTest(CoreDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task DefaultQueue_Should_NotThrow()
    {
        SubjectCourseModel subjectCourse = await _database.Context.SubjectCourses.FirstAsync();

        StudentGroupModel group = subjectCourse.SubjectCourseGroups
            .Select(x => x.StudentGroup)
            .First(group => subjectCourse.Assignments
                .SelectMany(x => x.GroupAssignments)
                .SelectMany(x => x.Submissions)
                .Any(x => x.Student.StudentGroup?.Equals(group) ?? false));

        SubmissionQueue queue = new DefaultQueueBuilder(group.Id, subjectCourse.Id).Build();

        var visitor = new FilterCriteriaVisitor(new SubmissionQuery.Builder());
        queue.AcceptFilterCriteriaVisitor(visitor);

        Submission[] submissions = await _database.PersistenceContext.Submissions
            .QueryAsync(visitor.Builder.Build(), default)
            .ToArrayAsync();

        submissions.Should().NotBeEmpty();
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}