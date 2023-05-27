using Bogus;

namespace ITMO.Dev.ASAP.Tests;

public class TestBase
{
    public TestBase()
    {
        Randomizer.Seed = new Random(101);
    }
}