using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.Dev.ASAP.Tests.Core;

public class CoreDatabaseTestBase : CoreTestBase, IAsyncLifetime
{
    private AsyncServiceScope _scope;

    protected CoreDatabaseTestBase(CoreDatabaseFixture fixture, ITestOutputHelper? output = null) : base(output)
    {
        Fixture = fixture;
    }

    protected DatabaseContext Context { get; private set; } = null!;

    protected IPersistenceContext PersistenceContext { get; private set; } = null!;

    protected CoreDatabaseFixture Fixture { get; }

    public async Task InitializeAsync()
    {
        _scope = Fixture.CreateAsyncScope();

        await _scope.UseDatabaseSeeders<TestDatabaseContext>();
        Context = _scope.ServiceProvider.GetRequiredService<TestDatabaseContext>();
        PersistenceContext = _scope.ServiceProvider.GetRequiredService<IPersistenceContext>();
    }

    public async Task DisposeAsync()
    {
        await Fixture.ResetAsync();
        await _scope.DisposeAsync();
    }

    protected T GetRequiredService<T>() where T : class
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }
}