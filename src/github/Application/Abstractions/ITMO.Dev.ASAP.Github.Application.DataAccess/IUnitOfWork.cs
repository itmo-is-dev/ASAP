namespace ITMO.Dev.ASAP.Github.Application.DataAccess;

public interface IUnitOfWork
{
    void Enqueue(string query, object arguments);

    Task CommitAsync(CancellationToken cancellationToken);
}