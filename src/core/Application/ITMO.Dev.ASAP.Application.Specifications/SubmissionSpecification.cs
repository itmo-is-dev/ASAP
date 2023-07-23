using ITMO.Dev.ASAP.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class SubmissionSpecification
{
    public static async Task<Submission> GetSubmissionForCodeOrLatestAsync(
        this ISubmissionRepository repository,
        Guid userId,
        Guid assignmentId,
        int? code,
        CancellationToken cancellationToken)
    {
        SubmissionQuery.Builder builder = new SubmissionQuery.Builder()
            .WithUserId(userId)
            .WithAssignmentId(assignmentId);

        builder = code is null
            ? builder.WithOrderByCode(OrderDirection.Descending).WithLimit(1)
            : builder.WithCode(code.Value);

        Submission? submission = await repository
            .QueryAsync(builder.Build(), cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return submission ?? throw new EntityNotFoundException("Could not find submission");
    }

    public static async Task<Submission> GetByIdAsync(
        this ISubmissionRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = SubmissionQuery.Build(x => x.WithId(id));

        Submission? submission = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return submission ?? throw EntityNotFoundException.For<Submission>(id);
    }
}