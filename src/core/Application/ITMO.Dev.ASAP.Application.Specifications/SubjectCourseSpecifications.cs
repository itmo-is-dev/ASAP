using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class SubjectCourseSpecifications
{
    public static async Task<SubjectCourse> GetByIdAsync(
        this ISubjectCourseRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = SubjectCourseQuery.Build(x => x.WithId(id));

        SubjectCourse? subjectCourse = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return subjectCourse ?? throw EntityNotFoundException.For<SubjectCourse>(id);
    }

    public static async Task<SubjectCourse> GetByAssignmentId(
        this ISubjectCourseRepository repository,
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        var query = SubjectCourseQuery.Build(x => x.WithAssignmentId(assignmentId));

        SubjectCourse? subjectCourse = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is not null)
            return subjectCourse;

        throw new EntityNotFoundException($"SubjectCourse for assignment {assignmentId} not found");
    }

    public static async Task<SubjectCourse> GetBySubmissionIdAsync(
        this ISubjectCourseRepository repository,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var query = SubjectCourseQuery.Build(x => x.WithSubmissionId(submissionId));

        SubjectCourse? subjectCourse = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is not null)
            return subjectCourse;

        throw new EntityNotFoundException($"SubjectCourse for submission {submissionId} not found");
    }
}