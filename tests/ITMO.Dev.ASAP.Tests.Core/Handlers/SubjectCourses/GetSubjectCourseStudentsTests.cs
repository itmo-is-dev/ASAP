using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Handlers.SubjectCourses;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class GetSubjectCourseStudentsTests : CoreDatabaseTestBase
{
    public GetSubjectCourseStudentsTests(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task HandleAsync_ShouldReturnSubjectCourseStudents()
    {
        // Arrange
        var subjectCourse = await Context.SubjectCourses
            .OrderBy(x => x.Id)
            .Select(subjectCourse => new
            {
                id = subjectCourse.Id,
                studentsCount = subjectCourse.SubjectCourseGroups
                    .SelectMany(x => x.StudentGroup.Students)
                    .Count(),
            })
            .Where(x => x.studentsCount != 0)
            .FirstAsync();

        var query = new GetSubjectCourseStudents.Query(subjectCourse.id);
        var handler = new GetSubjectCourseStudentsHandler(PersistenceContext);

        // Act
        GetSubjectCourseStudents.Response response = await handler.Handle(query, default);

        // Assert
        response.StudentIds.Should().HaveCount(subjectCourse.studentsCount);
    }
}