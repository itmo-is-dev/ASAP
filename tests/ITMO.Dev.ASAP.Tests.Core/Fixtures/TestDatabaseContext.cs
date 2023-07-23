using ITMO.Dev.ASAP.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Tests.Core.Fixtures;

public class TestDatabaseContext : DatabaseContext
{
    public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options) : base(options) { }
}