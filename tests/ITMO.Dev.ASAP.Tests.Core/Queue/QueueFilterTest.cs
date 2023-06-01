using FluentAssertions;
using ITMO.Dev.ASAP.Application.Tools;
using ITMO.Dev.ASAP.Domain.Queue;
using ITMO.Dev.ASAP.Domain.Queue.Building;
using ITMO.Dev.ASAP.Domain.Study;
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
        SubjectCourse subjectCourse = await _database.Context.SubjectCourses.FirstAsync();

        StudentGroup group = subjectCourse.Groups
            .Select(x => x.StudentGroup)
            .First(group => subjectCourse.Assignments
                .SelectMany(x => x.GroupAssignments)
                .SelectMany(x => x.Submissions)
                .Any(x => x.Student.Group?.Equals(group) ?? false));

        SubmissionQueue queue = new DefaultQueueBuilder(group, subjectCourse.Id).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            _database.Context.Submissions,
            new QueryExecutor(),
            default);

        submissions.Should().NotBeEmpty();
    }

    public Task DisposeAsync()
    {
        return _database.ResetAsync();
    }
}