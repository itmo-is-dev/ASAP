using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.DeveloperEnvironment;

public class DeveloperEnvironmentSeeder
{
    private const string ExceptedEnvironment = "Testing";

    private readonly IEntityGenerator<UserModel> _userGenerator;
    private readonly DatabaseContext _context;

    public DeveloperEnvironmentSeeder(
        DatabaseContext context,
        IEntityGenerator<UserModel> userGenerator)
    {
        _context = context;
        _userGenerator = userGenerator;
    }

    public async Task Handle(DeveloperEnvironmentSeedingRequest request, CancellationToken cancellationToken = default)
    {
        EnsureUserAcknowledgedEnvironment(request);
        AddUsers();

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureUserAcknowledgedEnvironment(DeveloperEnvironmentSeedingRequest request)
    {
        if (!request.Environment.Equals(ExceptedEnvironment, StringComparison.OrdinalIgnoreCase))
            throw new UserNotAcknowledgedEnvironmentException();
    }

    private void AddUsers()
    {
        IReadOnlyList<UserModel> users = _userGenerator.GeneratedEntities;
        _context.Users.AddRange(users);
    }
}