using System.Data;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess;

public interface IUnitOfWork
{
    void Enqueue(string query, object arguments);

    Task CommitAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);
}