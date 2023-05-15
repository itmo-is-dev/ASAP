using Dapper;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using System.Collections.Concurrent;
using System.Data;

namespace ITMO.Dev.ASAP.Github.DataAccess;

internal sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ConcurrentQueue<Work> _work;
    private readonly GithubDbConnection _connection;
    private readonly SemaphoreSlim _semaphore;

    public UnitOfWork(GithubDbConnection connection)
    {
        _connection = connection;
        _work = new ConcurrentQueue<Work>();
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public void Enqueue(string query, object arguments)
    {
        var work = new Work(query, arguments);
        _work.Enqueue(work);
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return CommitAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task CommitAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        IDbTransaction transaction = _connection.Connection.BeginTransaction(isolationLevel);
        int count = _work.Count;

        try
        {
            while (count is not 0 && _work.TryDequeue(out Work work))
            {
                count--;

                var definition = new CommandDefinition(
                    work.Query,
                    work.Arguments,
                    transaction: transaction,
                    cancellationToken: cancellationToken);

                await _connection.Connection.ExecuteAsync(definition);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
        finally
        {
            transaction.Dispose();

            while (count is not 0 && _work.TryDequeue(out _))
            {
                count--;
            }

            _semaphore.Release();
        }
    }

    private record struct Work(string Query, object Arguments);

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}