namespace ITMO.Dev.ASAP.Domain.Queue;

public interface IQueryExecutor
{
    Task<IReadOnlyCollection<T>> ExecuteAsync<T>(IQueryable<T> query, CancellationToken cancellationToken);
}