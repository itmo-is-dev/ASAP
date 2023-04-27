using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using User = ITMO.Dev.ASAP.Domain.Users.User;

namespace ITMO.Dev.ASAP.DeveloperEnvironment;

public class DeveloperEnvironmentSeeder
{
    private const string ExceptedEnvironment = "Testing";

    private readonly IDatabaseContext _context;
    private readonly IEntityGenerator<User> _userGenerator;

    public DeveloperEnvironmentSeeder(
        IDatabaseContext context,
        IEntityGenerator<User> userGenerator)
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
        IReadOnlyList<User> users = _userGenerator.GeneratedEntities;
        _context.Users.AttachRange(users);
    }
}