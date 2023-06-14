using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Groups;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class StudentGroupSpecifications
{
    public static async Task<StudentGroup> GetByIdAsync(
        this IStudentGroupRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = StudentGroupQuery.Build(x => x.WithId(id));

        StudentGroup? studentGroup = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return studentGroup ?? throw EntityNotFoundException.For<StudentGroup>(id);
    }

    public static async Task<StudentGroup> GetByStudentId(
        this IStudentGroupRepository repository,
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var query = StudentGroupQuery.Build(x => x.WithStudentId(studentId));

        StudentGroup? group = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (group is null)
            throw new EntityNotFoundException($"Group for student {studentId} not found");

        return group;
    }
}