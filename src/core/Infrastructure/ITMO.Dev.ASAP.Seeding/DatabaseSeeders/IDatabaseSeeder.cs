using ITMO.Dev.ASAP.DataAccess.Contexts;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public interface IDatabaseSeeder
{
    int Priority => 0;

    void Seed(DatabaseContext context);
}