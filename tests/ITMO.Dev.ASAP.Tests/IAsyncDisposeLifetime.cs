using Xunit;

namespace ITMO.Dev.ASAP.Tests;

public interface IAsyncDisposeLifetime : IAsyncLifetime
{
    Task IAsyncLifetime.InitializeAsync()
    {
        return Task.CompletedTask;
    }
}