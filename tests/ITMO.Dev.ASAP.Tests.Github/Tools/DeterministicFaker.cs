using Bogus;

namespace ITMO.Dev.ASAP.Tests.Github.Tools;

public class DeterministicFaker : Faker
{
    public DeterministicFaker()
    {
        Random = new Randomizer(0);
    }
}