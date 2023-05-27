using ITMO.Dev.ASAP.Application.DataAccess;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public interface IDatabaseSeeder
{
    int Priority => 0;

    void Seed(IDatabaseContext context);
}