using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Students;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class StudentSpecifications
{
    public static IAsyncEnumerable<Student> GetStudentsBySubjectCourseIdAsync(
        this IStudentRepository repository,
        Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        var query = StudentQuery.Build(x => x.WithSubjectCourseId(subjectCourseId));
        return repository.QueryAsync(query, cancellationToken);
    }

    public static async Task<Student> GetByIdAsync(
        this IStudentRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = StudentQuery.Build(x => x.WithId(id));

        Student? student = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return student ?? throw EntityNotFoundException.For<Student>(id);
    }
}